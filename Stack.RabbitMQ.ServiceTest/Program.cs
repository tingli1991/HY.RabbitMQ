using Stack.RabbitMQ.Extensions;
using System;
using System.IO;

namespace Stack.RabbitMQ.ServiceTest
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
            RabbitmqExtensions
                .Configure(Path.Combine(Directory.GetCurrentDirectory(), "Config"), "rabbitmq.json")
                .OnStart();

            Console.WriteLine("启动完成！！！");
            Console.ReadLine();
        }
    }
}