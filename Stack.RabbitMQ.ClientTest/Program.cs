using Stack.RabbitMQ.Enums;
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
                string fileDir = Path.Combine(Directory.GetCurrentDirectory(), "Config");
                RabbitmqBuilder.Configure(fileDir, "rabbitmq.json");

                var model = new
                {
                    Id = 001,
                    Content = "测试消息体"
                };
                var response = ProducerFactory.Execute(ExchangeType.Direct, model, "Exchange.Direct", "Exchange.Direct.Queue001");
                Console.WriteLine($"运行结果：{response}");
            }
            catch (Exception ex)
            {

            }
            Console.ReadLine();
        }
    }
}