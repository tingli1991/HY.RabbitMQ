namespace Stack.RabbitMQ.Options
{
    /// <summary>
    /// Mogodb数据库链接参数
    /// </summary>
    class MongoConnectionOptions
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        public string ConnectionString { get; set; }
    }
}