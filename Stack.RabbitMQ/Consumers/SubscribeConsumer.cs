using log4net;
using RabbitMQ.Client;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Options;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// 订阅模式消费
    /// </summary>
    class SubscribeConsumer : BaseConsumer
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<RoutingConsumer>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public SubscribeConsumer(IModel channel, RabbitmqServiceOptions config)
            : base(channel, config)
        {

        }
    }
}