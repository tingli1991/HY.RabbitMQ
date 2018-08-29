using RabbitMQ.Client;
using Stack.RabbitMQ.Result;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 消费者
    /// </summary>
    public class BaseProducer
    {
        /// <summary>
        /// 消息连接通道
        /// </summary>
        public IModel Channel { get; set; }

        /// <summary>
        /// 路由Key
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="imodel"></param>
        /// <param name="config"></param>
        public BaseProducer(string queueName, IModel imodel)
        {
            Channel = imodel;
            RoutingKey = queueName;
        }


        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="messageBody"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="durabled"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public virtual ResponseResult Execute(object messageBody, string exchangeName, string routingKey, bool durabled, ushort timeOut)
        {
            return null;
        }
    }
}