using RabbitMQ.Client;
using Stack.RabbitMQ.Enums;
using Stack.RabbitMQ.Extensions;
using System;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 发布订阅模式
    /// </summary>
    class PublishProducer : BaseProducer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="patternType"></param>
        public PublishProducer(PublishPatternType patternType) : base(patternType)
        {
        }

        /// <summary>
        /// 发布执行处理方法
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey"></param>
        /// <param name="messageBody">消息内容</param>
        /// <param name="publishTime">发布时间</param>
        /// <param name="durable">是否持久化</param>
        /// <param name="headers">头部信息</param>
        /// <returns></returns>
        public override ResponseResult Publish(string exchangeName, string routingKey, object messageBody, DateTime? publishTime, bool durable, Dictionary<string, object> headers = null)
        {
            ResponseResult result = new ResponseResult();
            if (string.IsNullOrEmpty(routingKey))
            {
                result.ErrorCode = "500";
                result.ErrorMsg = "参数routingKey必填";
                return result;
            }

            if (string.IsNullOrEmpty(exchangeName))
            {
                result.ErrorCode = "500";
                result.ErrorMsg = "参数exchangeName必填";
                return result;
            }

            exchangeName = PatternType.GetExchangeName(exchangeName);//解析交换机名称
            using (var channel = RabbitmqContext.Connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, "fanout");//声明交换机
                channel.QueueDeclare(routingKey, durable, false, false, arguments: null);//声明队列
                channel.QueueBind(routingKey, exchangeName, routingKey);//建立队列与交换机的绑定关系

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.MessageId = Guid.NewGuid().ToString("N");
                basicProperties.DeliveryMode = durable ? (byte)2 : (byte)1;//持久化方式
                basicProperties.Headers = headers ?? new Dictionary<string, object> { };
                basicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                if (publishTime.HasValue)
                {
                    DateTime ctime = DateTime.Now;
                    if (publishTime.Value < ctime)
                    {
                        result.ErrorCode = "500";
                        result.ErrorMsg = "发布时间必须要大于系统当前时间";
                        return result;
                    }
                    routingKey = routingKey.GetTaskRoutingKey();
                    exchangeName = RabbitmqContext.TaskExchangeName;
                    basicProperties.Expiration = ((int)publishTime.Value.Subtract(ctime).TotalMilliseconds).ToString();
                }
                channel.BasicPublish(exchangeName, routingKey, basicProperties, messageBody.ToBytes());
                result.Success = true;
            }
            return result;
        }
    }
}