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
        public string PluginPath { get; set; }

        /// <summary>
        /// 配置文件
        /// </summary>
        public ConsumerNodeConfig Config { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginPath"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public BaseConsumer(string pluginPath, IModel channel, ConsumerNodeConfig config)
        {
            Config = config;
            Channel = channel;
            PluginPath = pluginPath;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="connectionStrings">连接字符串</param>
        /// <param name="serviceConfiguration"></param>
        public virtual void Start() { }
    }
}