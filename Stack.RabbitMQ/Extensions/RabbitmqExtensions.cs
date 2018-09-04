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
        #region log4net
        /// <summary>
        /// 使用log4net记录日志
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static IHostBuilder UseLog4net(this IHostBuilder builder, string configPath)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Log4Context.Configure(configPath);
            return builder;
        }

        /// <summary>
        /// 使用log4net记录日志
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseLog4net(this IWebHostBuilder builder, string configPath)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            Log4Context.Configure(configPath);
            return builder;
        }
        #endregion

        #region 加载rabbitmq配置
        /// <summary>
        /// 加载rabbitmq配置
        /// 备注：无论是客户端还是服务端必须先加载配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IHostBuilder Configure(this IHostBuilder builder, string fileDir, string fileName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            RabbitmqContext.Configure(fileDir, fileName);
            return builder;
        }
        /// <summary>
        /// 加载rabbitmq配置
        /// 备注：无论是客户端还是服务端必须先加载配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IWebHostBuilder Configure(this IWebHostBuilder builder, string fileDir, string fileName)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            RabbitmqContext.Configure(fileDir, fileName);
            return builder;
        }
        #endregion

        #region 启用rabbitmq消费者业务主机
        /// <summary>
        /// 启用rabbitmq消费者业务主机
        /// 备注：如果是消费者类型的项目才需要启用该主机（例如：客户端的生产者则无需启用就可以正常使用）
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IHostBuilder UseBusinessHost(this IHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (RabbitmqContext.Config == null)
                throw new ArgumentNullException(nameof(RabbitmqContext));

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<BusinessHostService>();//业务主机服务
            });
            return builder;
        }
        /// <summary>
        /// 启用rabbitmq消费者业务主机
        /// 备注：如果是消费者类型的项目才需要启用该主机（例如：客户端的生产者则无需启用就可以正常使用）
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileDir">配置文件路径</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseBusinessHost(this IWebHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (RabbitmqContext.Config == null)
                throw new ArgumentNullException(nameof(RabbitmqContext));

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<BusinessHostService>();//业务主机服务
            });
            return builder;
        }
        #endregion

        #region 启用rabbitmq审计队列主机
        /// <summary>
        /// 启用审计队列主机
        /// 备注：rabbitmq审计队列主机是用来记录消费者业务主机每个消费者的请求参数；
        ///      该主机一旦启用，那么每个消被标记为需要审计的消费者的请求参数都将被记录到mogodb数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder UseAuditHost(this IHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (RabbitmqContext.Config == null)
                throw new ArgumentNullException(nameof(RabbitmqContext));

            builder.ConfigureServices((hostContext, services) =>
            {
                //添加Mogo数据库
                services.AddMongoContext();

                //审计主机服务
                services.AddHostedService<AuditHostService>();
            });
            return builder;
        }
        /// <summary>
        /// 启用审计队列主机
        /// 备注：rabbitmq审计队列主机是用来记录消费者业务主机每个消费者的请求参数；
        ///      该主机一旦启用，那么每个消被标记为需要审计的消费者的请求参数都将被记录到mogodb数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseAuditHost(this IWebHostBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (RabbitmqContext.Config == null)
                throw new ArgumentNullException(nameof(RabbitmqContext));

            builder.ConfigureServices((hostContext, services) =>
            {
                //添加Mogo数据库
                services.AddMongoContext();

                //审计主机服务
                services.AddHostedService<AuditHostService>();
            });
            return builder;
        }
        #endregion
    }
}