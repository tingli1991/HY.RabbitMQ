using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Stack.RabbitMQ.Context;
using System;
using System.IO;

namespace Stack.RabbitMQ.Extensions
{
    /// <summary>
    /// RabbitMQ扩展
    /// </summary>
    public static class RabbitmqExtensions
    {
        /// <summary>
        /// 使用log4net记录日志
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static RabbitmqBuilder UseLog4net(this RabbitmqBuilder builder, string configPath)
        {
            Log4Context.Configure(configPath);
            return builder;
        }

        /// <summary>
        /// 运行服务端
        /// </summary>
        /// <param name="connection"></param>
        public static void RunServiceHost(this RabbitmqBuilder connection)
        {
            new MQServiceHost().Run();
        }

        /// <summary>
        /// 设置配置文件
        /// 注意：该方法默认调用当前项目执行目录下面的log4net.config作为配置文件
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static void Configure(this IConfiguration configuration)
        {
            RabbitmqBuilder.Configure(Directory.GetCurrentDirectory(), "rabbitmq.json");
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void Configure(this IConfiguration configuration, string fileDir, string fileName)
        {
            RabbitmqBuilder.Configure(fileDir, fileName);
        }

        /// <summary>
        /// 使用NLog
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseNRabbitMQ(this IWebHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            RabbitmqBuilder.Configure(Directory.GetCurrentDirectory(), "rabbitmq.json");
            return builder;
        }

        /// <summary>
        ///  使用NLog
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseNRabbitMQ(this IWebHostBuilder builder, string fileDir, string fileName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            RabbitmqBuilder.Configure(fileDir, fileName);
            return builder;
        }
    }
}