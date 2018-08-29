namespace Stack.RabbitMQ.Enums
{
    /// <summary>
    /// 消费者类型
    /// </summary>
    public enum ExchangeType
    {
        RPC = 1,

        /// <summary>
        /// 直连交换器
        /// </summary>
        Direct = 2
    }
}