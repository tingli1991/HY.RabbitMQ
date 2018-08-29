using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Consumers;
using System;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// 队列服务主机
    /// </summary>
    public class MQServiceHost
    {
        /// <summary>
        /// 启动
        /// </summary>
        public void OnStart()
        {
            var config = RabbitmqBuilder.Config;
            if (config == null)
            {
                throw new TypeInitializationException("RabbitmqConfig", null);
            }
            
            var services = config.Consumer.Nodes;
            foreach (ConsumerNodeConfig service in services)
            {
                var channel = RabbitmqBuilder.ModelDic.GetOrAdd(service.QueueName, RabbitmqBuilder.Connection.CreateModel());
                var constructorArgs = new object[] { config.Consumer.PluginConfigPath, channel, service };
                var instance = ConsumerFactory.GetInstance(service.ExchangeType, constructorArgs);
                instance.OnStart();
            }
        }
    }
}