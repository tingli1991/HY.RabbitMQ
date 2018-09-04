using Microsoft.Extensions.Hosting;
using Stack.RabbitMQ.Extensions;
using System.IO;

namespace Stack.RabbitMQ.ServiceTest
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// 程序主函数
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
            .Configure(configDir, "rabbitmq.json")//加载配置文件
            .UseBusinessHost()//启用业务主机
            .UseAuditHost()//启用审计队列
            .Build();
    }
}