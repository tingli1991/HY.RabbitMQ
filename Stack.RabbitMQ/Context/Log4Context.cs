using log4net;
using log4net.Repository;
using System.IO;

namespace Stack.RabbitMQ.Context
{
    /// <summary>
    /// log4net上下文
    /// </summary>
    class Log4Context
    {
        /// <summary>
        /// 日志仓库
        /// </summary>
        private static readonly ILoggerRepository Repository = LogManager.CreateRepository("NETCoreRepository");

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="configPath"></param>
        public static void Configure(string configPath)
        {
            FileInfo file = new FileInfo(configPath);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(Repository, file);
        }

        /// <summary>
        /// 获取日志记录器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILog GetLogger<T>() where T : class
        {
            return LogManager.GetLogger(Repository.Name, typeof(T));
        }
    }
}