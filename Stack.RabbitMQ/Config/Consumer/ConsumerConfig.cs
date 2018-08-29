using System.Collections.Generic;
using System.IO;

namespace Stack.RabbitMQ.Config
{
    /// <summary>
    /// 消费者配置节点
    /// </summary>
    class ConsumerConfig
    {
        /// <summary>
        /// 是否使用绝对路径（默认：fasle表示相对路径）
        /// </summary>
        public bool AbsolutePath { get; set; } = false;

        /// <summary>
        /// 节点列表
        /// </summary>
        public List<ConsumerNodeConfig> Nodes { get; set; }

        /// <summary>
        /// 插件地址路径
        /// </summary>
        public string PluginPath { get; set; }

        /// <summary>
        /// 插件存放路径
        /// </summary>
        public string PluginConfigPath
        {
            get
            {
                string pluginPath = PluginPath;
                if (!AbsolutePath)
                {
                    string baseDir = Directory.GetCurrentDirectory();
                    pluginPath = Path.Combine(baseDir, PluginPath);
                }
                return pluginPath;
            }
        }
    }
}