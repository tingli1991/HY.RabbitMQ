using RabbitMQ.Client;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Utils;
using System;
using System.IO;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// RabbitMQ编译对象
    /// </summary>
    public sealed class RabbitmqBuilder
    {
        /// <summary>
        /// 配置文件初始化
        /// </summary>
        /// <param name="fileDir">文件目录</param>
        /// <param name="fileName">文件名称</param>
        public static RabbitmqBuilder Configure(string fileDir, string fileName)
        {
            var configPath = Path.Combine(fileDir, fileName);
            if (!File.Exists(configPath))
            {
                string errMsg = $"配置文件：{configPath}不存在！！！";
                throw new FileNotFoundException(errMsg);
            }

            //加载配置文件
            RabbitmqContext.Config = AppSettingsUtil.GetValue<RabbitmqConfig>(fileDir, fileName);
            if (RabbitmqContext.Config == null)
            {
                string errMsg = $"配置文件：{configPath}初始化异常！！！";
                throw new TypeInitializationException("RabbitmqConfig", null);
            }

            //创建链接工厂
            var connectionStrings = RabbitmqContext.Config.ConnectionStrings;
            RabbitmqContext.ConnectionFactory = new ConnectionFactory()
            {
                Port = connectionStrings.Port,
                AutomaticRecoveryEnabled = true,
                HostName = connectionStrings.Host,
                Password = connectionStrings.Password,
                UserName = connectionStrings.UserName,
                RequestedHeartbeat = connectionStrings.TimeOut
            };

            //创建链接
            RabbitmqContext.Connection = RabbitmqContext.ConnectionFactory.CreateConnection();
            return new RabbitmqBuilder();
        }
    }
}