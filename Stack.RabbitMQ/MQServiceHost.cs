using RabbitMQ.Client;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Utils;
using System;
using System.Collections.Generic;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// 队列服务主机
    /// </summary>
    public class MQServiceHost
    {
        /// <summary>
        /// 对象锁
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 消费者队列缓存
        /// </summary>
        private static Dictionary<string, IConsumer> InstanceDic = new Dictionary<string, IConsumer>();

        /// <summary>
        /// 启动
        /// </summary>
        public void OnStart()
        {
            var config = RabbitmqContext.Config;
            if (config == null)
            {
                throw new TypeInitializationException("RabbitmqConfig", null);
            }

            var services = config.ServiceNode.Services;
            foreach (ServiceConfig service in services)
            {
                var fullName = ReflectionUtil.GetFullName(service.NameSpace, service.ClassName);
                if (!InstanceDic.ContainsKey(fullName))
                {
                    lock (_lock)
                    {
                        if (!InstanceDic.ContainsKey(fullName))
                        {
                            var instance = (IConsumer)SingletonUtil.GetInstance(config.ServiceNode.PluginPath, service.AssemblyName, service.NameSpace, service.ClassName);
                            var model = RabbitmqContext.ModelDic.GetOrAdd(service.QueueName, RabbitmqContext.Connection.CreateModel());

                            //instance.OnStart(config.ConnectionStrings, service);
                            //instanceDic.Add(fullName, instance);
                        }
                    }
                }
            }
        }
    }
}