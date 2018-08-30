using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Services;
using System;

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
        public static IHostBuilder UseLog4net(this IHostBuilder builder, string configPath)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            Log4Context.Configure(configPath);
            return builder;
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IHostBuilder Configure(this IHostBuilder builder, string fileDir, string fileName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            RabbitmqContext.Configure(fileDir, fileName);
            return builder;
        }

        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IWebHostBuilder Configure(this IWebHostBuilder builder, string fileDir, string fileName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            RabbitmqContext.Configure(fileDir, fileName);
            return builder;
        }

        /// <summary>
        /// 使用RabbitMQ
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IHostBuilder UseRabbitMQ(this IHostBuilder builder, string fileDir, string fileName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            RabbitmqContext.Configure(fileDir, fileName);
            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<AuditHostService>();//审计主机服务
                services.AddHostedService<BusinessHostService>();//业务主机服务
            });
            return builder;
        }
    }
}