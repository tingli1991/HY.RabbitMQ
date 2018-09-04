using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Enums;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Producers;
using System;
using System.Collections.Generic;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// RabbitMQ客户端
    /// </summary>
    public sealed class RabbitMQClient
    {
        /// <summary>
        /// 确保线程同步的对象锁
        /// </summary>
        private static readonly object locker = new object();
        /// <summary>
        /// 缓存字典
        /// </summary>
        private static Dictionary<PublishPatternType, BaseProducer> InstanceCacheDic = new Dictionary<PublishPatternType, BaseProducer>();
        /// <summary>
        /// 根据点赞类型获取对象实例
        /// </summary>
        /// <param name="patternType">模式类型</param>
        /// <param name="constructorArgs">可变的构造函数列表</param>
        /// <returns></returns>
        private static BaseProducer GetInstance(PublishPatternType patternType, params object[] constructorArgs)
        {
            if (!InstanceCacheDic.ContainsKey(patternType))
            {
                lock (locker)
                {
                    if (!InstanceCacheDic.ContainsKey(patternType))
                    {
                        string assemblyName = "Stack.RabbitMQ.Producers";
                        string className = $"{assemblyName}.{patternType.ToString()}Producer";
                        BaseProducer instance = (BaseProducer)Activator.CreateInstance(Type.GetType(className), constructorArgs);
                        InstanceCacheDic.Add(patternType, instance);
                    }
                }
            }
            return InstanceCacheDic[patternType];
        }
        /// <summary>
        /// 拉取指定队列的消息
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <param name="autoAck">是否自动应答（true表示自动应答；false则需要手动应答）</param>
        /// <returns></returns>
        public static BasicGetResult Pull(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException("queueName");

            using (var channel = RabbitmqContext.Connection.CreateModel())
            {
                BasicGetResult result = channel.BasicGet(queueName, false);
                if (result != null)
                {
                    channel.BasicAck(result.DeliveryTag, false);
                }
                return result;
            }
        }
        /// <summary>
        /// 拉取指定队列的消息
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <returns></returns>
        public static void Pull(string queueName, Action<BasicGetResult> action)
        {
            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException("queueName");

            using (var channel = RabbitmqContext.Connection.CreateModel())
            {
                BasicGetResult result = channel.BasicGet(queueName, false);
                if (result != null)
                {
                    channel.BasicAck(result.DeliveryTag, false);
                }
                action(result);
            }
        }
        /// <summary>
        /// 拉取指定队列的消息
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <returns></returns>
        public static ResponseResult Pull(string queueName, Func<BasicGetResult, ResponseResult> func)
        {
            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException("queueName");

            using (var channel = RabbitmqContext.Connection.CreateModel())
            {
                BasicGetResult result = channel.BasicGet(queueName, false);
                if (result != null)
                {
                    channel.BasicAck(result.DeliveryTag, false);
                }
                return func(result);
            }
        }
        /// <summary>
        /// 客户端订阅
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="queueName">队列名称</param>
        /// <returns></returns>
        public static void Subscribe(string exchangeName, Action<ConsumerContext> action)
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException("exchangeName");

            using (var channel = RabbitmqContext.Connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, "fanout");
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, args) =>
                {
                    ConsumerContext context = args.GetConsumerContext();
                    action(context);
                };
                channel.BasicConsume(queueName, true, consumer);
            }
        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="patternType">模式类型</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="messageBody">消息内容</param>
        /// <param name="publishTime">发布时间(到了发布时间队列才会执行)</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="durabled">是否持久化(默认：需要持久化)</param>
        /// <param name="headers">头部信息</param>
        /// <returns></returns>
        public static ResponseResult Publish(PublishPatternType patternType, string queueName = "", object messageBody = null, DateTime? publishTime = null,
            string exchangeName = "",  bool durabled = true, Dictionary<string, object> headers = null)
        {
            var instance = GetInstance(patternType, patternType);
            return instance.Publish(exchangeName, queueName, messageBody, publishTime, durabled, headers);
        }
    }
}