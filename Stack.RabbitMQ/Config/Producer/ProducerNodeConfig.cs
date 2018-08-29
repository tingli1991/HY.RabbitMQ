using Stack.RabbitMQ.Enums;

namespace Stack.RabbitMQ.Config
{
    /// <summary>
    /// 消费者配置
    /// </summary>
    public class ProducerNodeConfig
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 插件的程序集
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 插件的命名空间
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// 消费者的类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 是否持久化（默认：true表示需要持久化）
        /// </summary>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// 消费者类型
        /// </summary>
        public ExchangeType ConsumerType { get; set; }
    }
}