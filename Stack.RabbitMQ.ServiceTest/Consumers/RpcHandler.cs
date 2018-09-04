namespace Stack.RabbitMQ.ServiceTest.Consumers
{
    /// <summary>
    /// 短信消费者
    /// </summary>
    public class RpcHandler : IConsumer
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
                Data = "测试访问成功的结果数据"
            };
        }
    }
}