using Stack.RabbitMQ.Param;
using Stack.RabbitMQ.Result;

namespace Stack.RabbitMQ.ServiceTest.Consumers
{
    /// <summary>
    /// 短信消费者
    /// </summary>
    public class SmsConsumer : IConsumer
    {
        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ResponseResult Handler(ConsumerContext context)
        {
            return new ResponseResult()
            {
                Success = true,
                Data = "测试访问成功的结果数据"
            };
        }
    }
}