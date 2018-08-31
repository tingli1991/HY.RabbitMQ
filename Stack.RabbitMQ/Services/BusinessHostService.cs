using log4net;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Factory;
using System;
using System.Collections.Concurrent;
using System.Linq;
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
        /// RabbitMQ建议客户端线程之间不要共用Model，至少要保证共用Model的线程发送消息必须是串行的，但是建议尽量共用Connection
        /// </summary>
        public static readonly ConcurrentDictionary<string, IModel> ChannelDic = new ConcurrentDictionary<string, IModel>();
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
                var channel = ChannelDic.GetOrAdd(node.QueueName, RabbitmqContext.Connection.CreateModel());
                var constructorArgs = new object[] { config.Consumer.PluginConfigPath, channel, node };
                ConsumerFactory.GetInstance(node.PatternType, constructorArgs).Start();
                _log.Info($"【业务主机】队列：{node.QueueName}启动成功！！！");
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
            Dispose();//释放资源
            _log.Info($"【业务主机】停止完成！！！");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (ChannelDic != null && ChannelDic.Any())
            {
                foreach (var channel in ChannelDic.Values)
                {
                    channel?.Close();
                    channel?.Dispose();
                }
            }
            _log.Info($"【业务主机】资源释放完成！！！");
        }
    }
}