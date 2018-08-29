using Stack.RabbitMQ.Enums;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Config
{
    /// <summary>
    /// 服务配置
    /// </summary>
    public class ConsumerNodeConfig
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
        /// 交换机类型
        /// </summary>
        public ExchangeType ExchangeType { get; set; }

        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// 路由键
        /// </summary>
        public List<string> RoutingKeys { get; set; }
    }
}