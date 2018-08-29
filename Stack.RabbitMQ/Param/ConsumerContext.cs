using Stack.RabbitMQ.Config;

namespace Stack.RabbitMQ.Param
{
    /// <summary>
    /// 消费者上下文
    /// </summary>
    public class ConsumerContext
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public dynamic Body { get; set; }

        /// <summary>
        /// 消息内容，文件流格式
        /// </summary>
        public byte[] BodyBytes { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public ConsumerNodeConfig Config { get; set; }

        /// <summary>
        /// 插件配置路径
        /// </summary>
        public string PluginConfigPath { get; set; }
    }
}