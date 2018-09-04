using Stack.RabbitMQ.Enums;

namespace Stack.RabbitMQ.Producers
{
    /// <summary>
    /// 交换机生产模式
    /// </summary>
    class TopicProducer : BaseProducer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="imodel"></param>
        /// <param name="channel"></param>
        public TopicProducer(PublishPatternType patternType)
            : base(patternType)
        {

        }
    }
}