using Stack.RabbitMQ.Enums;
using Stack.RabbitMQ.Result;
using System;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class ProducerFactory
    {
        /// <summary>
        /// 确保线程同步的对象锁
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// 缓存字典
        /// </summary>
        private static Dictionary<ExchangeType, BaseProducer> InstanceCacheDic = new Dictionary<ExchangeType, BaseProducer>();

        /// <summary>
        /// 根据点赞类型获取对象实例
        /// </summary>
        /// <param name="exchangeType">消费者类型</param>
        /// <param name="constructorArgs">可变的构造函数列表</param>
        /// <returns></returns>
        private static BaseProducer GetInstance(ExchangeType exchangeType, params object[] constructorArgs)
        {
            if (!InstanceCacheDic.ContainsKey(exchangeType))
            {
                lock (locker)
                {
                    if (!InstanceCacheDic.ContainsKey(exchangeType))
                    {
                        string assemblyName = "Stack.RabbitMQ.Producers";
                        string className = $"{assemblyName}.{exchangeType.ToString()}Producer";
                        BaseProducer instance = (BaseProducer)Activator.CreateInstance(Type.GetType(className), constructorArgs);
                        InstanceCacheDic.Add(exchangeType, instance);
                    }
                }
            }
            return InstanceCacheDic[exchangeType];
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="exchangeType">交换机类型</param>
        /// <param name="messageBody">消息内容</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">路由Key</param>
        /// <param name="durabled">是否持久化</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        public static ResponseResult Execute(ExchangeType exchangeType, object messageBody, string exchangeName = "", string routingKey = "", bool durabled = true, ushort timeOut = 30)
        {
            var model = RabbitmqContext.ChannelDic.GetOrAdd(routingKey, RabbitmqContext.Connection.CreateModel());
            var instance = GetInstance(exchangeType, routingKey, model);
            return instance.Execute(messageBody, exchangeName, routingKey, durabled, timeOut);
        }
    }
}