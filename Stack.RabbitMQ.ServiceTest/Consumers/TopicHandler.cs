namespace Stack.RabbitMQ.ServiceTest.Consumers
{
    /// <summary>
    /// 
    /// </summary>
    public class TopicHandler : IConsumer
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
                Data = "Topic测试数据返回结果！！！"
            };
        }
    }
}