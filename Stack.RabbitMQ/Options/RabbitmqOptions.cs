using System.Collections.Generic;
using System.IO;

namespace Stack.RabbitMQ.Options
{
    /// <summary>
    /// RabbitMQ配置选项
    /// </summary>
    class RabbitmqOptions
    {
        /// <summary>
        /// 插件地址路径
        /// </summary>
        public string PluginDir { get; set; }

        /// <summary>
        /// 是否使用绝对路径（默认：fasle表示相对路径）
        /// </summary>
        public bool AbsolutePath { get; set; } = false;

        /// <summary>
        /// 插件存放路径
        /// </summary>
        public string PluginPath
        {
            get
            {
                string pluginPath = PluginDir ?? "";
                if (!AbsolutePath)
                {
                    string baseDir = Directory.GetCurrentDirectory();
                    pluginPath = Path.Combine(baseDir, pluginPath);
                }
                return pluginPath;
            }
        }

        /// <summary>
        /// 服务端节点配置
        /// </summary>
        public List<RabbitmqServiceOptions> Services { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public RabbitConnectionOptions ConnectionString { get; set; }

        /// <summary>
        /// Mongo数据库链接字符串
        /// </summary>
        public MongoConnectionOptions MongoConnectionString { get; set; }
    }
}