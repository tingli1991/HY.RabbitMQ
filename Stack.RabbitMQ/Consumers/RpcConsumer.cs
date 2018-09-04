using log4net;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Options;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// RPC消费者模式
    /// </summary>
    class RPCConsumer : BaseConsumer
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<RPCConsumer>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public RPCConsumer(IModel channel, RabbitmqServiceOptions config)
            : base(channel, config)
        {

        }

        /// <summary>
        /// 业务处理成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result">业务处理结果</param>
        /// <param name="eventArgs"></param>
        protected override void SuccessHandler(object sender, ResponseResult result, BasicDeliverEventArgs eventArgs)
        {
            //回传给RPC客户端临时队列
            var properties = eventArgs.BasicProperties;
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.CorrelationId = properties.CorrelationId;
            _channel.BasicPublish("", properties.ReplyTo, basicProperties, body: result.ToBytes());
        }

        /// <summary>
        /// 重试业务处理方法
        /// </summary>
        /// <param name="retryCount">当前进行第几次重试</param>
        /// <param name="retryRoutingKey">重试队列路由Key</param>
        /// <param name="eventArgs"></param>
        /// <returns>是否应答，true:应答，false：不应答</returns>
        protected override bool RetryHandler(int retryCount, string retryRoutingKey, BasicDeliverEventArgs eventArgs)
        {
            bool canAck = false;//是否应答
            var properties = eventArgs.BasicProperties;
            if (_retryTimeRules == null || _retryTimeRules.Count <= 0 || (retryCount > _retryTimeRules.Count))
            {
                var basicProperties = _channel.CreateBasicProperties();
                basicProperties.CorrelationId = properties.CorrelationId;
                ResponseResult result = new ResponseResult() { Success = false, ErrorMsg = "业务处理失败！！！" };
                _channel.BasicPublish("", properties.ReplyTo, basicProperties, body: result.ToBytes());
                return canAck;
            }

            int index = retryCount - 1;
            properties.Headers[RabbitmqContext.RetryCountKeyName] = retryCount;
            properties.Expiration = _retryTimeRules[index].ToString();
            properties.Headers = properties.Headers ?? new Dictionary<string, object>();
            try
            {
                _channel.BasicPublish(RabbitmqContext.TaskExchangeName, retryRoutingKey, properties, eventArgs.Body);
                canAck = true;//重试一旦发出则标记为应答
            }
            catch (AlreadyClosedException ex)
            {
                _log.Error($"MessageId：{eventArgs.BasicProperties.MessageId}重试发生异常(RabbitMQ is Closed)：", ex);
            }
            return canAck;
        }
    }
}