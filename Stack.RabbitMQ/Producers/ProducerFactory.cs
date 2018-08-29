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
        /// <param name="consumerType">消费者类型</param>
        /// <param name="constructorArgs">可变的构造函数列表</param>
        /// <returns></returns>
        private static BaseProducer GetInstance(ExchangeType consumerType, params object[] constructorArgs)
        {
            if (!InstanceCacheDic.ContainsKey(consumerType))
            {
                lock (locker)
                {
                    if (!InstanceCacheDic.ContainsKey(consumerType))
                    {
                        string assemblyName = "Stack.RabbitMQ.Producers";
                        string className = $"{assemblyName}.{consumerType.ToString()}Producer";
                        BaseProducer instance = (BaseProducer)Activator.CreateInstance(Type.GetType(className), constructorArgs);
                        InstanceCacheDic.Add(consumerType, instance);
                    }
                }
            }
            return InstanceCacheDic[consumerType];
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="consumerType"></param>
        /// <param name="messageBody"></param>
        /// <param name="exchangeName"></param>
        /// <param name="durabled"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static ResponseResult Execute(string routingKey, ExchangeType consumerType, object messageBody, string exchangeName, bool durabled = true, ushort timeOut = 30)
        {
            var model = RabbitmqBuilder.ModelDic.GetOrAdd(routingKey, RabbitmqBuilder.Connection.CreateModel());
            var instance = GetInstance(consumerType, routingKey, model);
            return instance.Execute(messageBody, exchangeName, routingKey, durabled, timeOut);
        }
    }
}