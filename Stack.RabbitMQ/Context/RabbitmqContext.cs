using RabbitMQ.Client;
using Stack.RabbitMQ.Config;
using System.Collections.Concurrent;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// RabbitMQ上下文
    /// </summary>
    sealed class RabbitmqContext
    {
        /// <summary>
        /// 私有构造函数，禁止外部使用new关键字创建对象
        /// </summary>
        private RabbitmqContext() { }
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
    }
}