using Stack.RabbitMQ.Enums;
using System;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// 消费者工厂
    /// </summary>
    class ConsumerFactory
    {
        /// <summary>
        /// 确保线程同步的对象锁
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// 缓存字典
        /// </summary>
        private static Dictionary<PatternType, BaseConsumer> InstanceCacheDic = new Dictionary<PatternType, BaseConsumer>();

        /// <summary>
        /// 根据点赞类型获取对象实例
        /// </summary>
        /// <param name="patternType">消费者类型</param>
        /// <param name="constructorArgs">可变的构造函数列表</param>
        /// <returns></returns>
        public static BaseConsumer GetInstance(PatternType patternType, params object[] constructorArgs)
        {
            if (!InstanceCacheDic.ContainsKey(patternType))
            {
                lock (locker)
                {
                    if (!InstanceCacheDic.ContainsKey(patternType))
                    {
                        string assemblyName = "Stack.RabbitMQ.Consumers";
                        string className = $"{assemblyName}.{patternType.ToString()}Consumer";
                        BaseConsumer instance = (BaseConsumer)Activator.CreateInstance(Type.GetType(className), constructorArgs);
                        InstanceCacheDic.Add(patternType, instance);
                    }
                }
            }
            return InstanceCacheDic[patternType];
        }
    }
}