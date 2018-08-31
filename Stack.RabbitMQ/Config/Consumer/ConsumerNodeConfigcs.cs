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
        /// 模式类型
        /// </summary>
        public PatternType PatternType { get; set; }

        /// <summary>
        /// 重试的时间规则（单位：秒）
        /// </summary>
        public List<int> RetryTimeRules { get; set; }

        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName => $"Stack.RabbitMQ.{PatternType}.{ExchangeType}";

        /// <summary>
        /// 重试交换机
        /// </summary>
        public string RetryExchangeName => $"Stack.RabbitMQ.{PatternType}.{ExchangeType}.Retry";
    }
}