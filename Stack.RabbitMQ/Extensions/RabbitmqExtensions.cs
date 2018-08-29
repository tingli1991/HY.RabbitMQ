using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
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
        /// 设置配置文件
        /// 注意：该方法默认调用当前项目执行目录下面的log4net.config作为配置文件
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static void Configure(this IConfiguration configuration)
        {
            Configure();
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
            Configure(fileDir, fileName);
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

            Configure();
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
            Configure(fileDir, fileName);
            return builder;
        }

        /// <summary>
        /// 设置配置文件
        /// 注意：该方法默认调用当前项目执行目录下面的log4net.config作为配置文件
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConnection Configure()
        {
            return Configure(Directory.GetCurrentDirectory(), "rabbitmq.json");
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileDir">配置文件路径</param
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static IConnection Configure(string fileDir, string fileName)
        {
            return RabbitmqBuilder.Configure(fileDir, fileName);
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="connection"></param>
        public static void OnStart(this IConnection connection)
        {
            new MQServiceHost().OnStart();
        }
    }
}