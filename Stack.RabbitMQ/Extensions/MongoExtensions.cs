using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using Stack.RabbitMQ.Context;
using System;

namespace Stack.RabbitMQ.Extensions
{
    /// <summary>
    /// MongoDB扩展类
    /// </summary>
    static class MongoExtensions
    {
        /// <summary>
        /// 添加Mongodb上下文配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoContext(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (RabbitmqContext.Config?.MongoConnectionString == null)
                throw new ArgumentNullException(nameof(RabbitmqContext));

            services.AddOptions();
            var options = RabbitmqContext.Config.MongoConnectionString;
            services.Add(ServiceDescriptor.Singleton(new MongoContext(options)));
            SetConvention();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SetConvention()
        {
            var pack = new ConventionPack()
            {
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("IgnoreExtraElements&IgnoreIfNull", pack, type => true);
        }
    }
}