using RabbitMQ.Client;
using Stack.RabbitMQ.Config;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// 基础消费者
    /// </summary>
    class BaseConsumer
    {
        /// <summary>
        /// 消息连接通道
        /// </summary>
        public IModel Channel { get; set; }

        /// <summary>
        /// 插件配置路径
        /// </summary>
        public string PluginConfigPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConsumerNodeConfig Config { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginConfigPath"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public BaseConsumer(string pluginConfigPath, IModel channel, ConsumerNodeConfig config)
        {
            Config = config;
            Channel = channel;
            PluginConfigPath = pluginConfigPath;
        }

        /// <summary>
        /// 运行消费者
        /// </summary>
        /// <param name="connectionStrings">连接字符串</param>
        /// <param name="serviceConfiguration"></param>
        public virtual void Run() { }
    }
}