using Microsoft.Extensions.Hosting;
using Stack.RabbitMQ.Enums;
using System;
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

            var publishTime = DateTime.Now.AddHours(1);
            for (int i = 0; i < 100000; i++)
            {
                //var response = RabbitMQClient.Publish(PublishPatternType.RPC, messageBody, routingKey: "queue.rpc.rpctesthandler");
                var response1 = RabbitMQClient.Publish(PublishPatternType.Routing, messageBody, publishTime: publishTime, routingKey: "queue.direct.directtesthandler");
                //var response3 = RabbitMQClient.Publish(PublishPatternType.Topic, messageBody, routingKey: "queue.topic.topictesthandler");
                //var response3 = RabbitMQClient.Pull("queue.direct.directtesthandler");
                if (i % 100 == 0)
                {
                    System.Console.Write(".");
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}