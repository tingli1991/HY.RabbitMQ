using log4net;
using Microsoft.Extensions.Hosting;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Consumers;
using Stack.RabbitMQ.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stack.RabbitMQ.Services
{
    /// <summary>
    /// 业务主机服务
    /// </summary>
    public class BusinessHostService : IHostedService, IDisposable
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<BusinessHostService>();

        /// <summary>
        /// 启动服务
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = RabbitmqContext.Config;
            if (config == null)
            {
                throw new TypeInitializationException("RabbitmqConfig", null);
            }

            foreach (ConsumerNodeConfig node in config.Consumer.Nodes)
            {
                var channel = RabbitmqContext.ChannelDic.GetOrAdd(node.QueueName, RabbitmqContext.Connection.CreateModel());
                var constructorArgs = new object[] { config.Consumer.PluginConfigPath, channel, node };
                ConsumerFactory.GetInstance(node.PatternType, constructorArgs).Run();
                _log.Info($"队列：{node.QueueName}启动成功！！！");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 服务停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {

        }
    }
}