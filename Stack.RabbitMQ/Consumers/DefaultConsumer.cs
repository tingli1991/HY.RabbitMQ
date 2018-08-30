using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Utils;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// 默认的消费者模式
    /// </summary>
    class DefaultConsumer : BaseConsumer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginConfigPath"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public DefaultConsumer(string pluginConfigPath, IModel channel, ConsumerNodeConfig config)
            : base(pluginConfigPath, channel, config)
        {

        }

        /// <summary>
        /// 运行
        /// </summary>
        public override void Run()
        {
            Channel.ExchangeDeclare(exchange: Config.ExchangeName, type: "direct");//声明交换机
            Channel.QueueDeclare(queue: Config.QueueName, durable: Config.Durable, exclusive: false, autoDelete: false, arguments: null);//声明队列
            Channel.QueueBind(queue: Config.QueueName, exchange: Config.ExchangeName, routingKey: Config.QueueName);//建立队列与交换机的绑定关系

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, args) =>
            {
                var instance = (IConsumer)SingletonUtil.GetInstance(PluginConfigPath, Config.AssemblyName, Config.NameSpace, Config.ClassName);
                var response = instance.Handler(new Param.ConsumerContext()
                {
                    BodyBytes = args.Body,
                    Body = args.Body.ToObject()
                });
            };
            Channel.BasicConsume(queue: Config.QueueName, autoAck: false, consumer: consumer);
        }
    }
}
