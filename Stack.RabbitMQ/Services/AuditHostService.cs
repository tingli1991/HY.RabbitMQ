using log4net;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stack.RabbitMQ.Services
{
    /// <summary>
    /// 审计主机服务
    /// </summary>
    class AuditHostService : IHostedService, IDisposable
    {
        /// <summary>
        /// 通信渠道
        /// </summary>
        private IModel _channel;
        /// <summary>
        /// MongoDb上下文
        /// </summary>
        private readonly MongoContext _mongoDbContext;
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<AuditHostService>();
        /// <summary>
        /// 构造函数
        /// </summary>
        public AuditHostService(MongoContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _channel = RabbitmqContext.Connection.CreateModel();
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

            _channel.BasicQos(0, 1, false);
            _channel.QueueDeclare(RabbitmqContext.AuditQueueName, true, false, false, null);//声明审计队列
            foreach (RabbitmqServiceOptions service in config.Services)
            {
                if (!service.IsAudit)
                    continue;

                var exchangeType = service.PatternType.GetExchangeType();
                var exchangeName = service.PatternType.GetExchangeName(service.ExchangeName);
                _channel.ExchangeDeclare(exchangeName, exchangeType);//申明交换机
                _channel.QueueBind(RabbitmqContext.AuditQueueName, exchangeName, service.QueueName);//建立队列与交换机的绑定关系
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var canAck = true;
                try
                {
                    IBasicProperties basicProperties = eventArgs.BasicProperties;//消息属性
                    IDictionary<string, object> headers = eventArgs.BasicProperties.Headers;//头部信息
                    if (!string.IsNullOrEmpty(eventArgs.Exchange) && (headers == null || !headers.ContainsKey(RabbitmqContext.RetryCountKeyName)))
                    {
                        ConsumerContext context = eventArgs.GetConsumerContext();//获取消费者消息处理上下文
                        _mongoDbContext.Collection<ConsumerContext>().InsertOneAsync(context, cancellationToken: cancellationToken);//添加审计记录
                    }
                }
                catch (Exception ex)
                {
                    canAck = false;
                    _log.Error($"【审计日志】事件参数：{JsonConvert.SerializeObject(eventArgs)}", ex);
                }
                //处理应答结果
                AnswerHandler(canAck, eventArgs);
            };
            _channel.BasicConsume(RabbitmqContext.AuditQueueName, false, consumer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 应答处理
        /// </summary>
        /// <param name="canAck"></param>
        /// <param name="eventArgs"></param>
        /// <returns>应答处理结果，true：表示成功，false：表示失败</returns>
        protected virtual void AnswerHandler(bool canAck, BasicDeliverEventArgs eventArgs)
        {
            try
            {
                ulong deliveryTag = eventArgs.DeliveryTag;
                if (canAck)
                {
                    _channel.BasicAck(deliveryTag, false);
                }
                else
                {
                    _channel.BasicNack(deliveryTag, false, false);
                }
            }
            catch (AlreadyClosedException ex)
            {
                string messageId = eventArgs.BasicProperties.MessageId;
                _log.Error($"MessageId：{messageId}，重试发生异常(RabbitMQ is Closed)：", ex);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();//释放资源
            _log.Info($"【审计主机】停止完成！！！");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
        }
    }
}