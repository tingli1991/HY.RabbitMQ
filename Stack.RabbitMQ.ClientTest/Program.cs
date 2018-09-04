using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stack.RabbitMQ.Extensions;
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
            string baseDir = Directory.GetCurrentDirectory();
            string configDir = Path.Combine(baseDir, "Config");
            IHost host = CreateDefaultHost(configDir);//创建主机
            host.Run();
        }

        /// <summary>
        /// 创建编译
        /// </summary>
        /// <param name="configDir"></param>
        /// <returns></returns>
        static IHost CreateDefaultHost(string configDir) => new HostBuilder()
            .UseLog4net(Path.Combine(configDir, "log4net.config"))
            .Configure(configDir, "rabbitmq.json")
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<TestHostService>();
            })
            .Build();
    }
}