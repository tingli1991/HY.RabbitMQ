using Stack.RabbitMQ.Enums;
using Stack.RabbitMQ.Extensions;
using Stack.RabbitMQ.Producers;
using System;
using System.IO;

namespace Stack.RabbitMQ.ClientTest
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                RabbitmqExtensions
               .Configure(Path.Combine(Directory.GetCurrentDirectory(), "Config"), "rabbitmq.json");

                var model = new
                {
                    Id = 001,
                    Content = "测试消息体"
                };

                var response = ProducerFactory.Execute("Queue.Rpc.SmsConsumer", ExchangeType.RPC, model, "Exchange.Direct");
                Console.WriteLine($"运行结果：{response}");
            }
            catch (Exception ex)
            {

            }
            Console.ReadLine();
        }
    }
}