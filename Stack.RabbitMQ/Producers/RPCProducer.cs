using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Enums;
using Stack.RabbitMQ.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 
    /// </summary>
    class RPCProducer : BaseProducer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="patternType"></param>
        public RPCProducer(PublishPatternType patternType)
            : base(patternType)
        {

        }

        /// <summary>
        /// 发布执行处理方法
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey"></param>
        /// <param name="messageBody"></param>
        /// <param name="publishTime">发布时间</param>
        /// <param name="durable"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public override ResponseResult Publish(string exchangeName, string routingKey, object messageBody, DateTime? publishTime, bool durable, Dictionary<string, object> headers = null)
        {
            ResponseResult result = new ResponseResult();
            if (string.IsNullOrEmpty(routingKey))
            {
                result.ErrorCode = "500";
                result.ErrorMsg = "参数queueName必填";
                return result;
            }

            if (string.IsNullOrEmpty(exchangeName))
            {
                result.ErrorCode = "500";
                result.ErrorMsg = "参数exchangeName必填";
                return result;
            }

            if (publishTime.HasValue)
            {
                result.ErrorCode = "500";
                result.ErrorMsg = "Rpc模式下不支持,也不建议定时发送";
                return result;
            }

            exchangeName = PatternType.GetExchangeName(exchangeName);//解析交换机名称
            using (var channel = RabbitmqContext.Connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);//消息接受事件
                var replyQueueName = channel.QueueDeclare().QueueName;//回复的队列名称

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.ReplyTo = replyQueueName;//指定回复的队列名称
                basicProperties.MessageId = Guid.NewGuid().ToString("N");
                basicProperties.DeliveryMode = durable ? (byte)2 : (byte)1;//持久化方式
                basicProperties.CorrelationId = Guid.NewGuid().ToString("N");//关联Id
                basicProperties.Headers = headers ?? new Dictionary<string, object> { };
                basicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                BlockingCollection<ResponseResult> resposeQueue = new BlockingCollection<ResponseResult>();
                consumer.Received += (sender, eventArgs) =>
                            {
                                if (eventArgs.BasicProperties.CorrelationId == basicProperties.CorrelationId)
                                {
                                    var bodyBytes = eventArgs.Body;
                                    var response = bodyBytes.ToObject<ResponseResult>();
                                    resposeQueue.Add(response);
                                }
                            };
                channel.BasicConsume(replyQueueName, true, consumer);
                channel.BasicPublish(exchangeName, routingKey ?? "", basicProperties, messageBody.ToBytes());
                return resposeQueue.Take();
            }
        }
    }
}