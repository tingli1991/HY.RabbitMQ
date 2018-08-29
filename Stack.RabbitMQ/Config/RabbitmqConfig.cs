namespace Stack.RabbitMQ.Config
{
    /// <summary>
    /// RabbitMQ配置文件
    /// </summary>
    class RabbitmqConfig
    {
        /// <summary>
        /// 服务端配置节点
        /// </summary>
        public ConsumerConfig Consumer { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public ConnectionStringsConfig ConnectionStrings { get; set; }
    }
}