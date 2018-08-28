using System.Collections.Generic;
using System.IO;

namespace Stack.RabbitMQ.Config
{
    /// <summary>
    /// 服务端配置节点
    /// </summary>
    public class ServiceNodeConfig
    {
        /// <summary>
        /// 是否使用绝对路径（默认：fasle表示相对路径）
        /// </summary>
        public bool AbsolutePath { get; set; } = false;

        /// <summary>
        /// 服务列表
        /// </summary>
        public List<ServiceConfig> Services { get; set; }

        /// <summary>
        /// 插件存放路径
        /// </summary>
        public string PluginPath { get; set; } = Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// 服务配置
    /// </summary>
    public class ServiceConfig
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
    }
}