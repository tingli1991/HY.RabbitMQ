namespace Stack.RabbitMQ.Enums
{
    /// <summary>
    /// 交换器类型
    /// </summary>
    public enum ExchangeType
    {
        /// <summary>
        /// 直接交换器
        /// </summary>
        Direct = 1,
        /// <summary>
        /// 广播式式交换器
        /// </summary>
        Fanout = 2,
        /// <summary>
        /// headers类型交换器，不依赖routing key与binding key的匹配规则来路由消息
        /// </summary>
        Headers = 3,
        /// <summary>
        /// 主题交换器
        /// </summary>
        Topic = 4
    }
}