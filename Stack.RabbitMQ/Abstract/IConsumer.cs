using Stack.RabbitMQ.Param;
using Stack.RabbitMQ.Result;

namespace Stack.RabbitMQ
{
    /// <summary>
    /// 消费者接口
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// 队列处理方法
        /// </summary>
        ResponseResult Handler(ConsumerContext context);
    }
}