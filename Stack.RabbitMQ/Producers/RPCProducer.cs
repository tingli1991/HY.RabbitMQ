using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Result;
using System;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 
    /// </summary>
    public class RPCProducer : BaseProducer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public RPCProducer(string queueName, IModel channel)
            : base(queueName, channel)
        {

        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageBody"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="consumerType"></param>
        /// <param name="durabled"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public override ResponseResult Execute(object messageBody, string exchangeName, string routingKey, bool durabled, ushort timeOut)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                DateTime date = DateTime.Now;
                var millisecondsTimeout = timeOut * 1000;//超时毫秒数

                var consumer = new QueueingBasicConsumer(Channel);
                var replyQueueName = Channel.QueueDeclare().QueueName;//回复的队列名称
                var basicProperties = Channel.CreateBasicProperties();
                basicProperties.ReplyTo = replyQueueName;//指定回复的队列名称
                basicProperties.CorrelationId = Guid.NewGuid().ToString("N");//关联Id
                basicProperties.DeliveryMode = durabled ? (byte)2 : (byte)1;//持久化方式

                Channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
                Channel.BasicPublish(exchange: "", routingKey: RoutingKey, basicProperties: basicProperties, body: messageBody.ToBytes());
                if (consumer.Queue.Dequeue(millisecondsTimeout, out BasicDeliverEventArgs eventArgs))
                {
                    if (eventArgs.BasicProperties.CorrelationId == basicProperties.CorrelationId)
                    {
                        var bodyBytes = eventArgs.Body;
                        result = bodyBytes.ToObject<ResponseResult>();
                        return result;
                    }
                }

                var diff = DateTime.Now - date;
                if (eventArgs == null || diff.TotalMilliseconds > millisecondsTimeout)
                {
                    throw new Exception($"路由Key：{RoutingKey},等待：{diff.TotalMilliseconds}毫秒后超时！！！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}