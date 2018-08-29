using log4net;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Consumers;
using Stack.RabbitMQ.Context;
using System;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// 队列服务主机
    /// </summary>
    sealed class MQServiceHost
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<MQServiceHost>();

        /// <summary>
        /// 启动
        /// </summary>
        public void Run()
        {
            var config = RabbitmqContext.Config;
            if (config == null)
            {
                throw new TypeInitializationException("RabbitmqConfig", null);
            }

            var services = config.Consumer.Nodes;
            foreach (ConsumerNodeConfig service in services)
            {
                var channel = RabbitmqContext.ModelDic.GetOrAdd(service.QueueName, RabbitmqContext.Connection.CreateModel());
                var constructorArgs = new object[] { config.Consumer.PluginConfigPath, channel, service };
                ConsumerFactory.GetInstance(service.ExchangeType, constructorArgs).Run();
                _log.Info($"队列：{service.QueueName}运行成功！！！");
            }
        }
    }
}