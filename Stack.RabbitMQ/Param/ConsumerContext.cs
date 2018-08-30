using System;
using System.Collections.Generic;

namespace Stack.RabbitMQ.Param
{
    /// <summary>
    /// 消费者上下文
    /// </summary>
    public class ConsumerContext
    {
        /// <summary>
        /// 交换器名称
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// 路由key
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public dynamic Body { get; set; }

        /// <summary>
        /// 消息内容，文件流格式
        /// </summary>
        public byte[] BodyBytes { get; set; }

        /// <summary>
        /// 消息Id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long TimestampUnix { get; set; }

        /// <summary>
        /// 头部信息
        /// </summary>
        public IDictionary<string, object> Headers { get; set; }
    }
}