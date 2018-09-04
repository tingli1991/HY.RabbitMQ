namespace Stack.RabbitMQ.ServiceTest.Consumers
{
    /// <summary>
    /// Direct消费者测试类
    /// </summary>
    public class RoutingHandler : IConsumer
    {
        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ResponseResult Handler(ConsumerContext context)
        {
            var number = 0;
            var number1 = 1 / number;
            return new ResponseResult()
            {
                Success = true,
                Data = "Direct测试数据返回结果！！！"
            };
        }
    }
}