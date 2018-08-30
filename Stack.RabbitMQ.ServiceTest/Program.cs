﻿using Microsoft.Extensions.Hosting;
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
            string baseDir = Directory.GetCurrentDirectory();
            string configDir = Path.Combine(baseDir, "Config");
            IHost host = CreateDefaultHost(configDir);//创建主机
            Console.WriteLine("启动完成！！！");
            host.Run();
        }

        /// <summary>
        /// 创建编译
        /// </summary>
        /// <param name="configDir"></param>
        /// <returns></returns>
        static IHost CreateDefaultHost(string configDir) => new HostBuilder()
            .UseLog4net(Path.Combine(configDir, "log4net.config"))
            .UseRabbitMQ(configDir, "rabbitmq.json")
            .Build();
    }
}