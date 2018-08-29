using RabbitMQ.Client;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Result;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 
    /// </summary>
    public class DirectProducer : BaseProducer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public DirectProducer(string queueName, IModel channel)
            : base(queueName, channel)
        {

        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageBody"></param>
        /// <param name="exchangeName"></param>
        /// <param name="routingKey"></param>
        /// <param name="consumerType"></param>
        /// <param name="durabled"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public override ResponseResult Execute(object messageBody, string exchangeName, string routingKey, bool durabled, ushort timeOut)
        {
            Channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
            Channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: messageBody.ToBytes());
            return new ResponseResult() { Success = true };
        }
    }
}