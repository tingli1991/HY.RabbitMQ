using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Utils;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// 直连交换器
    /// </summary>
    class DirectConsumer : BaseConsumer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginConfigPath"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public DirectConsumer(string pluginConfigPath, IModel channel, ConsumerNodeConfig config)
            : base(pluginConfigPath, channel, config)
        {

        }

        /// <summary>
        /// 运行
        /// </summary>
        public override void Run()
        {
            Channel.ExchangeDeclare(exchange: Config.ExchangeName, type: "direct");//声明交换机
            QueueDeclareOk queueDeclare = Channel.QueueDeclare(queue: Config.QueueName, durable: Config.Durable, exclusive: false, autoDelete: false, arguments: null);
            var queueName = queueDeclare.QueueName;//当前队列的队列名称
            foreach (var routingKey in Config.RoutingKeys)
            {
                //将交换机跟队列建立绑定关系
                Channel.QueueBind(queue: queueName, exchange: Config.ExchangeName, routingKey: routingKey);
            }

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, args) =>
            {
                var instance = (IConsumer)SingletonUtil.GetInstance(PluginConfigPath, Config.AssemblyName, Config.NameSpace, Config.ClassName);
                var response = instance.Handler(new Param.ConsumerContext()
                {
                    Config = Config,
                    BodyBytes = args.Body,
                    Body = args.Body.ToObject(),
                    PluginConfigPath = PluginConfigPath
                });
            };
            Channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
    }
}
