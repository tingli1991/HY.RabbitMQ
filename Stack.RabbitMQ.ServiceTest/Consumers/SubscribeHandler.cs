namespace Stack.RabbitMQ.ServiceTest.Consumers
{
    /// <summary>
    /// 服务端订阅测试服务
    /// </summary>
    public class SubscribeHandler : IConsumer
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
                Data = $"路由Key:{context.RoutingKey}Fanout测试数据返回结果！！！"
            };
        }
    }
}