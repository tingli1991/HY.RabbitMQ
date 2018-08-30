using log4net;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Param;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stack.RabbitMQ.Services
{
    /// <summary>
    /// 审计主机服务
    /// </summary>
    public class AuditHostService : IHostedService, IDisposable
    {
        /// <summary>
        /// 通信渠道
        /// </summary>
        private IModel channel;
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<AuditHostService>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuditHostService()
        {
            string key = Guid.NewGuid().ToString("N");
            channel = RabbitmqContext.ChannelDic.GetOrAdd(key, RabbitmqContext.Connection.CreateModel());
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = RabbitmqContext.Config;
            if (config == null)
            {
                throw new TypeInitializationException("RabbitmqConfig", null);
            }

            channel.BasicQos(0, 1, false);
            var auditQueueName = $"Stack.RabbitMQ.AuditQueue";//固定声明审计队列名称
            channel.QueueDeclare(auditQueueName, true, false, false, null);//声明审计队列
            foreach (ConsumerNodeConfig node in config.Consumer.Nodes)
            {
                channel.ExchangeDeclare(node.ExchangeName, node.ExchangeType.ToString().ToLower());//申明交换机
                channel.QueueBind(auditQueueName, node.ExchangeName, node.QueueName);//建立队列与交换机的绑定关系
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var isOk = true;
                try
                {
                    IBasicProperties basicProperties = args.BasicProperties;//消息属性
                    IDictionary<string, object> headers = args.BasicProperties.Headers;//头部信息
                    if (headers == null || !headers.ContainsKey("x-death"))
                    {
                        var context = new ConsumerContext
                        {
                            ExchangeName = args.Exchange,
                            RoutingKey = args.RoutingKey,
                            BodyBytes = args.Body,
                            Body = args.Body.ToObject(),
                            MessageId = args.BasicProperties.MessageId
                        };

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

                        var unixTime = basicProperties.Timestamp.UnixTime;
                        if (unixTime > 0)
                        {
                            context.TimestampUnix = unixTime;
                            context.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(unixTime).UtcDateTime;
                        }

                        //添加审计记录
                        _log.Info($"【审计日志】交换器：{args.Exchange}，Context：{JsonConvert.SerializeObject(context)}");
                    }
                }
                catch (Exception ex)
                {
                    isOk = false;
                    _log.Error($"【审计异常】事件参数：{JsonConvert.SerializeObject(args)}", ex);
                }

                try
                {
                    if (isOk)
                    {
                        channel.BasicAck(args.DeliveryTag, false);
                    }
                    else
                    {
                        channel.BasicNack(args.DeliveryTag, false, true);
                    }
                }
                catch (AlreadyClosedException ex)
                {
                    _log.Error("RabbitMQ is closed!!!", ex);
                }
            };
            channel.BasicConsume(auditQueueName, false, consumer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止服务
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
            channel?.Close();
            channel?.Dispose();
        }
    }
}