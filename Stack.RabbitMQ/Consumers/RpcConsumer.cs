using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Utils;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// RPC消费者模式
    /// </summary>
    class RpcConsumer : BaseConsumer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginConfigPath"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public RpcConsumer(string pluginConfigPath, IModel channel, ConsumerNodeConfig config)
            : base(pluginConfigPath, channel, config)
        {

        }

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            Channel.QueueDeclare(queue: Config.QueueName, durable: Config.Durable, exclusive: false, autoDelete: false, arguments: null);
            Channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(Channel);//消息接受事件
            consumer.Received += (sender, args) =>
            {
                var properties = args.BasicProperties;
                var replyProperties = Channel.CreateBasicProperties();
                replyProperties.CorrelationId = properties.CorrelationId;
                Channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);

                var instance = (IConsumer)SingletonUtil.GetInstance(PluginPath, Config.AssemblyName, Config.NameSpace, Config.ClassName);
                var response = instance.Handler(new Param.ConsumerContext()
                {
                    BodyBytes = args.Body,
                    Body = args.Body.ToObject()
                });
                Channel.BasicPublish(exchange: "", routingKey: properties.ReplyTo, basicProperties: replyProperties, body: response.ToBytes());
            };
            Channel.BasicConsume(queue: Config.QueueName, autoAck: false, consumer: consumer);
        }
    }
}