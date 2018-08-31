using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Param;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stack.RabbitMQ.Extensions
{
    /// <summary>
    /// 基础投递扩展类
    /// </summary>
    public static class BasicDeliverExtensions
    {
        /// <summary>
        /// 获取消费者处理方法上下文
        /// </summary>
        /// <param name="args">包含有关从AMQP代理传递的消息的所有信息。</param>
        /// <returns></returns>
        public static ConsumerContext GetConsumerContext(this BasicDeliverEventArgs args)
        {
            var context = new ConsumerContext
            {
                BodyBytes = args.Body,
                Body = args.Body.ToObject(),
                ExchangeName = args.Exchange,
                RoutingKey = args.RoutingKey,
                MessageId = args.BasicProperties.MessageId
            };
            var headers = args.BasicProperties.Headers;//头部信息
            var unixTime = args.BasicProperties.Timestamp.UnixTime;
            if (unixTime > 0)
            {
                context.TimestampUnix = unixTime;
                context.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
            }
            if (headers != null)
            {
                context.Headers = new Dictionary<string, object>();
                foreach (var header in headers)
                {
                    if (header.Value is byte[] bytes)
                    {
                        context.Headers.Add(header.Key, Encoding.UTF8.GetString(bytes));
                    }
                    else
                    {
                        context.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            return context;
        }
    }
}