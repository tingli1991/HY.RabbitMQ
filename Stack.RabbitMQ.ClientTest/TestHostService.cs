using Microsoft.Extensions.Hosting;
using Stack.RabbitMQ.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stack.RabbitMQ.ClientTest
{
    /// <summary>
    /// 测试主机
    /// </summary>
    public class TestHostService : IHostedService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var messageBody = new
            {
                Id = 001,
                Name = "张三"
            };
            var headers = new Dictionary<string, object>()
            {
                { "TestKey001","测试头部Key001"},
                { "TestKey002","测试头部Key002"}
            };
            var publishTime = DateTime.Now.AddMinutes(1);
            for (int i = 0; i < 1000; i++)
            {
                //路由模式测试示例
                string routingKey1 = "queue.direct.routinghandler";//路由key
                var response0 = RabbitMQClient.Publish(PublishPatternType.Routing, routingKey1);//实时发送，服务端实施进行消费
                var response1 = RabbitMQClient.Publish(PublishPatternType.Routing, routingKey1, messageBody);//实时发送，服务端实施进行消费
                var response2 = RabbitMQClient.Publish(PublishPatternType.Routing, routingKey1, messageBody, publishTime: publishTime);//按指定时间发送，服务端会在指定的时间进行消费
                var response3 = RabbitMQClient.Publish(PublishPatternType.Routing, routingKey1, messageBody, headers: headers);//实时发送，服务端实施进行消费,带自定义头部信息，服务端handler的context.Headers会接收到headers参数

                //主题模式测试示例
                string routingKey2 = "queue.topic.topichandler";//路由key
                var response4 = RabbitMQClient.Publish(PublishPatternType.Topic, routingKey2, messageBody);//实时发送，服务端实施进行消费
                var response5 = RabbitMQClient.Publish(PublishPatternType.Topic, routingKey2, messageBody, publishTime: publishTime);//按指定时间发送，服务端会在指定的时间进行消费
                var response6 = RabbitMQClient.Publish(PublishPatternType.Topic, routingKey2, messageBody, headers: headers);//实时发送，服务端实施进行消费,带自定义头部信息，服务端handler的context.Headers会接收到headers参数

                //订阅模式测试示例(publish发布订阅消息)
                string exchangeName1 = "stack.rabbitmq.subscriexchange";//路由key
                var response7 = RabbitMQClient.Publish(PublishPatternType.Publish, messageBody: messageBody, exchangeName: exchangeName1);//实时发送，服务端实施进行消费
                var response8 = RabbitMQClient.Publish(PublishPatternType.Publish, messageBody: messageBody, exchangeName: exchangeName1, publishTime: publishTime);//按指定时间发送，服务端会在指定的时间进行消费
                var response9 = RabbitMQClient.Publish(PublishPatternType.Publish, messageBody: messageBody, exchangeName: exchangeName1, headers: headers);//实时发送，服务端实施进行消费,带自定义头部信息，服务端handler的context.Headers会接收到headers参数

                //RPC模式测试示例(由于RPC模式需要时时等待消费结果，所以对于RPC的模式来说就没有定时发送的功能)
                string routingKey4 = "queue.rpc.rpchandler";//路由key
                var response10 = RabbitMQClient.Publish(PublishPatternType.RPC, routingKey4, messageBody);//实时发送，服务端实施进行消费
                var response11 = RabbitMQClient.Publish(PublishPatternType.RPC, routingKey4, messageBody, headers: headers);//实时发送，服务端实施进行消费,带自定义头部信息，服务端handler的context.Headers会接收到headers参数

                //被动从指定队列拉取消息示例(获取到消息后会自动应答（所谓自动应答就是该消息去出来后会从队列消失）)
                string queueName5 = "queue.direct.routinghandler";//队列名称
                var response12 = RabbitMQClient.Pull(queueName5);//按顺序直接从队列中获取一条消息，直接返回消息
                RabbitMQClient.Pull(queueName5, message =>//按顺序直接从队列中获取一条消息，返回消息后可以对消息进行二次加工处理
                {
                    //加工处理的业务代码
                    var body = message?.Body;
                });
                var response13 = RabbitMQClient.Pull(queueName5, message =>//按顺序直接从队列中获取一条消息，返回消息后可以对消息进行二次加工处理，并返回处理结果
                {
                    //加工处理的业务代码
                    var body = message?.Body;
                    return new ResponseResult()
                    {
                        Success = true,
                        ErrorMsg = "业务处理成功"
                    };
                });

                //客户端订阅消息（由于考虑到性能问题，这中订阅模式，没有重试机制，目前也没想到那些场景会使用到重试，有想法欢迎大家积极反馈，合理的一定新增相关功能）
                string exchangeName = "stack.rabbitmq.subscribehandler";
                RabbitMQClient.Subscribe(exchangeName, context =>
                {
                    //订阅成功，执行后续的业务代码
                });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止服务主机
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}