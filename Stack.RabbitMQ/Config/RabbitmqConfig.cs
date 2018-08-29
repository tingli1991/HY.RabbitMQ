using System.Collections.Generic;

namespace Stack.RabbitMQ.Config
{
    /// <summary>
    /// RabbitMQ配置文件
    /// </summary>
    public class RabbitmqConfig
    {
        /// <summary>
        /// 队列列表
        /// </summary>
        public List<QueueConfig> Queues { get; set; }

        /// <summary>
        /// 服务端配置节点
        /// </summary>
        public ConsumerConfig Consumer { get; set; }

        /// <summary>
        /// 消费者
        /// </summary>
        public ProducerConfig Producer { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public ConnectionStringsConfig ConnectionStrings { get; set; }
    }
}