using RabbitMQ.Client;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Utils;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// RabbitMQ上下文
    /// </summary>
    public class RabbitmqContext
    {
        /// <summary>
        /// Socket链接
        /// </summary>
        public static IConnection Connection;

        /// <summary>
        /// 配置文件
        /// </summary>
        public static RabbitmqConfig Config = null;

        /// <summary>
        /// 链接工厂
        /// </summary>
        public static ConnectionFactory ConnectionFactory;

        /// <summary>
        /// RabbitMQ建议客户端线程之间不要共用Model，至少要保证共用Model的线程发送消息必须是串行的，但是建议尽量共用Connection
        /// </summary>
        public static readonly ConcurrentDictionary<string, IModel> ModelDic = new ConcurrentDictionary<string, IModel>();

        /// <summary>
        /// 配置文件初始化
        /// </summary>
        /// <param name="fileDir">文件目录</param>
        /// <param name="fileName">文件名称</param>
        internal static void Configure(string fileDir, string fileName)
        {
            var configPath = Path.Combine(fileDir, fileName);
            if (!File.Exists(configPath))
            {
                string errMsg = $"配置文件：{configPath}不存在！！！";
                throw new FileNotFoundException(errMsg);
            }

            //加载配置文件
            Config = AppSettingsUtil.GetValue<RabbitmqConfig>(fileDir, fileName);
            if (Config == null)
            {
                string errMsg = $"配置文件：{configPath}初始化异常！！！";
                throw new TypeInitializationException("RabbitmqConfig", null);
            }

            //创建链接工厂
            var connectionStrings = Config.ConnectionStrings;
            ConnectionFactory = new ConnectionFactory()
            {
                Port = connectionStrings.Port,
                AutomaticRecoveryEnabled = true,
                HostName = connectionStrings.Host,
                Password = connectionStrings.Password,
                UserName = connectionStrings.UserName,
                RequestedHeartbeat = connectionStrings.TimeOut
            };

            //创建链接
            Connection = ConnectionFactory.CreateConnection();
        }
    }
}