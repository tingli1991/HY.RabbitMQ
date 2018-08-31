using log4net;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Stack.RabbitMQ.Config;
using Stack.RabbitMQ.Context;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Param;
using Stack.RabbitMQ.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stack.RabbitMQ.Consumers
{
    /// <summary>
    /// 默认的消费者模式
    /// </summary>
    class DefaultConsumer : BaseConsumer
    {
        /// <summary>
        /// 重试队列名称
        /// </summary>
        private static string RetryQueueName = "";
        /// <summary>
        /// 第几次重试变量名称
        /// </summary>
        private const string RetryCountKeyName = "retryCount";
        /// <summary>
        /// 重试的时间规则
        /// </summary>
        private static List<int> RetryTimeRules = new List<int>();
        /// <summary>
        /// 日志记录器
        /// </summary>
        private static readonly ILog _log = Log4Context.GetLogger<DefaultConsumer>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pluginConfigPath"></param>
        /// <param name="channel"></param>
        /// <param name="config"></param>
        public DefaultConsumer(string pluginConfigPath, IModel channel, ConsumerNodeConfig config)
            : base(pluginConfigPath, channel, config)
        {
            RetryQueueName = $"{ Config.QueueName}.Retry";//重试队列名称
            RetryTimeRules = config?.RetryTimeRules?.Select(p => p * 1000).OrderBy(p => p).ToList();//加载重试的时间规则
        }

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            Channel.BasicQos(0, 1, false);//设置预取消息数量为1条
            string exchangeTypeStr = Config.ExchangeType.ToString().ToLower();//交换机类型字符串
            Channel.ExchangeDeclare(Config.ExchangeName, exchangeTypeStr);//声明交换机
            Channel.QueueDeclare(Config.QueueName, Config.Durable, false, false, arguments: null);//声明队列
            Channel.QueueBind(Config.QueueName, Config.ExchangeName, Config.QueueName);//建立队列与交换机的绑定关系

            var retryDic = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange",Config.ExchangeName},
                {"x-dead-letter-routing-key", Config.QueueName}
            };
            Channel.ExchangeDeclare(Config.RetryExchangeName, exchangeTypeStr);
            Channel.QueueDeclare(RetryQueueName, true, false, false, retryDic);
            Channel.QueueBind(RetryQueueName, Config.RetryExchangeName, RetryQueueName);

            var consumer = new EventingBasicConsumer(Channel);//定义消息接收回调
            consumer.Received += Consumer_Received;//回调接收处理
            Channel.BasicConsume(Config.QueueName, false, consumer);
        }

        /// <summary>
        /// 回调接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Consumer_Received(object sender, BasicDeliverEventArgs args)
        {
            bool canAck = false;
            int retryCount = 0;//第几次重试
            string messageId = args.BasicProperties.MessageId;//消息Id
            IDictionary<string, object> headers = args.BasicProperties.Headers;//头部信息
            if (headers != null && headers.ContainsKey(RetryCountKeyName))
            {
                retryCount = (int)headers[RetryCountKeyName] + 1;
                _log.Warn($"队列：{Config.QueueName}，MessageId：{messageId}，第：{retryCount}次重试开始！！！");
            }

            try
            {
                ConsumerContext context = args.GetConsumerContext();//获取消费者消息处理上下文
                var instance = (IConsumer)SingletonUtil.GetInstance(PluginPath, Config.AssemblyName, Config.NameSpace, Config.ClassName);
                instance.Handler(context);
                canAck = true;
            }
            catch (Exception ex)
            {
                _log.Error($"队列：{Config.QueueName}，MessageId：{messageId}，第：{retryCount + 1}次消费发生异常：", ex);
                canAck = RetryHandler(retryCount, Config.RetryExchangeName, RetryQueueName, args);
            }

            try
            {
                if (canAck)
                {

                    Channel.BasicAck(args.DeliveryTag, false);
                }
                else
                {
                    Channel.BasicNack(args.DeliveryTag, false, false);
                }
            }
            catch (AlreadyClosedException ex)
            {
                _log.Error("RabbitMQ is Closed：", ex);
            }
        }

        /// <summary>
        /// 异常重试业务处理方法
        /// </summary>
        /// <param name="retryCount">当前进行第几次重试</param>
        /// <param name="retryExchangeName">重试交换机名称</param>
        /// <param name="retryRoutingKey">重试队列路由Key</param>
        /// <param name="args"></param>
        /// <returns>是否应答，true:应答，false：不应答</returns>
        private bool RetryHandler(int retryCount, string retryExchangeName, string retryRoutingKey, BasicDeliverEventArgs args)
        {
            bool isAck = false;//是否应答
            if (retryCount <= RetryTimeRules.Count - 1)
            {
                var properties = args.BasicProperties;
                properties.Headers[RetryCountKeyName] = retryCount;
                properties.Expiration = RetryTimeRules[retryCount].ToString();
                properties.Headers = properties.Headers ?? new Dictionary<string, object>();
                try
                {
                    Channel.BasicPublish(retryExchangeName, retryRoutingKey, properties, args.Body);
                }
                catch (AlreadyClosedException ex)
                {
                    _log.Error("RabbitMQ is Closed：", ex);
                }
            }
            return isAck;
        }
    }
}