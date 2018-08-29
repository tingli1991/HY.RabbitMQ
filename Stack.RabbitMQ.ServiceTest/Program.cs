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
            string fileDir = Path.Combine(Directory.GetCurrentDirectory(), "Config");
            RabbitmqBuilder.Configure(fileDir, "rabbitmq.json")
                .UseLog4net(Path.Combine(fileDir, "log4net.config"))
                .RunServiceHost();

            Console.WriteLine("启动完成！！！");
            Console.ReadLine();
        }
    }
}