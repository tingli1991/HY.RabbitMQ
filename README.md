# Stack.RabbitMQ  
Stack.RabbitMQ 是一个对Rabbitmq进行二次封装的组件，目的在于提升开发效率，提高业务的高可用以及降低开发成本，本文只对本插件的使用做介绍，有不了解rabbitmq是什么(或者不熟悉的朋友)，可以先移步[基础教程篇](https://github.com/tingli1991/Stack.RabbitMQ/blob/master/%E5%9F%BA%E7%A1%80%E6%95%99%E7%A8%8B.md)进行了解，然后再来试着使用本插件   

### 使用之前需要了解的内容 
* **模式类型：** 所谓的模式类型就是组件Stack.RabbitMQ自身扩展的几种固定的生产和消费类型，每个模式的不同所支持的业务以及功能也有所不同，具体每种模式都具备什么功能，见下表：      

|          模式类型         |                            功能描述                                                                                 |
|---------------------------|---------------------------------------------------------------------------------------------------------------------|
| 路由模式(routing,direct)  | 1.消息定时发送(指定消息的发送时间);<br/>2.自定义消息重试功能(设置重试次数，设置重试时间间隔)                        |
| rpc模式(header)           | 1.自定义消息重试功能(设置重试次数，设置重试时间间隔)                                                                |
| 发布订阅模式(fanout)      | 1.消息定时发送(指定消息的发送时间);<br/>2.自定义消息重试功能(设置重试次数，设置重试时间间隔)                        |
| 主题模式(topic)           | 1.消息定时发送(指定消息的发送时间);<br/>2.自定义消息重试功能(设置重试次数，设置重试时间间隔)                        |
| 其他功能                  | 1.客户端订阅功能;<br/>2.客户端被动拉取指定队列消息(所谓被动就是用户在需要的时候才从队列去获取数据，在此称之为被动)  |



## Stack.RabbitMQ 消费者（服务端）    
#### 消费者（服务端）配置介绍  

|          配置名称         |是否必填|                            配置说明                                                                              |
|---------------------------|--------|------------------------------------------------------------------------------------------------------------------|
| PluginDir                 | 否     | 消费者插件存放目录（默认：不指定则标识程序的当前执行目录）                                                       |
| AbsolutePath              | 否     | 插件路径是否需要使用绝对路径(默认：相对路径，如果为true则标识绝对路口经)                                         |
| Services                  | 是     | 消费者队列列表                                                                                                   |
| Services.QueueName        | 是     | 消费者队列名称                                                                                                   |
| Services.AssemblyName     | 是     | 插件的程序集（获取消费者处理类的时候会从Services.AssemblyName.dll中获取）                                        |
| Services.NameSpace        | 是     | 插件的命名空间                                                                                                   |
| Services.ClassName        | 是     | 消费者业务处理类的类名                                                                                           |
| Services.Durable          | 否     | 队列是否需要持久化（默认：true表示需要持久化）                                                                   |
| Services.IsAudit          | 否     | 是否需要就有审计队列记录请求参数（该字段为true并且需要启动审计队列才有效）                                       |
| Services.ExchangeName     | 否     | 交换机名称（该字段配置只有在消费者为订阅模式的时候才生效），其他模式已经更具类型固定好交换机，无需使用者过多考虑 |
| Services.RetryTimeRules   | 否     | 重试时间间隔以及次数配置（条数表示重试次数、每项值表示间隔时间，不设置则不会启用重试机制）                       |
| Services.PatternType      | 是     | 模式类型：现目前支持4中（分别是：路由模式、rpc模式、订阅模式以及主题模式）                                       |
| ConnectionString          | 是     | rabbitmq的链接字符串                                                                                             |
| MongoConnectionString     | 否     | Mongo的链接字符串（注意：该配置只有在启用审计队列主机的情况下才生效，目前只用于记录审计日志）                    |  
``` javascript
{
  "PluginDir": "",
  "AbsolutePath": false,
  "ConnectionString": {
    "Port": 5672,
    "TimeOut": 60,
    "UserName": "admin",
    "Host": "192.168.3.10",
    "Password": "ChinaNet910111"
  },
  "MongoConnectionString": {
    "DatabaseName": "RabbitmqAudit",
    "ConnectionString": "mongodb://192.168.3.10:27017"
  },
  "Services": [
    {
      "Durable": true,
      "IsAudit": true,
      "PatternType": "Routing",
      "RetryTimeRules": [ 1, 30, 10 ],
      "QueueName": "queue.direct.routinghandler",
      "AssemblyName": "Stack.RabbitMQ.ServiceTest",
      "NameSpace": "Stack.RabbitMQ.ServiceTest.Consumers",
      "ClassName": "DirecttHandler"
    },
    {
      "Durable": true,
      "IsAudit": true,
      "PatternType": "RPC",
      "RetryTimeRules": [ 1, 30, 10 ],
      "QueueName": "queue.rpc.rpcHandler",
      "AssemblyName": "Stack.RabbitMQ.ServiceTest",
      "NameSpace": "Stack.RabbitMQ.ServiceTest.Consumers",
      "ClassName": "RpcHandler"
    },
    {
      "Durable": true,
      "IsAudit": true,
      "PatternType": "Subscribe",
      "RetryTimeRules": [ 1, 30, 10 ],
      "QueueName": "stack.rabbitmq.subscribehandler",
      "ExchangeName": "stack.rabbitmq.subscribehandler",
      "AssemblyName": "Stack.RabbitMQ.ServiceTest",
      "NameSpace": "Stack.RabbitMQ.ServiceTest.Consumers",
      "ClassName": "SubscribeHandler"
    },
    {
      "Durable": true,
      "IsAudit": true,
      "PatternType": "Topic",
      "RetryTimeRules": [ 1, 30, 10 ],
      "QueueName": "queue.topic.topicHandler",
      "AssemblyName": "Stack.RabbitMQ.ServiceTest",
      "NameSpace": "Stack.RabbitMQ.ServiceTest.Consumers",
      "ClassName": "TopicHandler"
    }
  ]
}
```  
#### 使用步骤  
在使用之前我们应为我们的项目需要一台宿主机，如果是控制台的话我们需要安装nuget工具包**Microsoft.Extensions.Hosting**，安装完成之后就可以安装如下的步骤执行即可（由于需求的不同，建议大家按需选择即可）
* **步骤一：环境初始化**  该步骤主要就是我们在程序启动的时候要制定rabbitmq所使用的配置和功能，具体见下面表格及示例代码：  

|          方法名称         |是否必填|                            方法说明                                                                                          |
|---------------------------|--------|------------------------------------------------------------------------------------------------------------------------------|
| Configure                 | 是     | 加载rabbitmq配置,这一步就是指定rabbitmq的环境配置，必不可少                                                                  |
| UseBusinessHost           | 是     | 启用rabbitmq消费者业务主机<br/>备注：如果是消费者类型的项目才需要启用该主机（例如：客户端的生产者则无需启用就可以正常使用）  |
| UseAuditHost              | 否     | 启用审计队列主机<br/>备注：rabbitmq审计队列主机是用来记录消费者业务主机每个消费者的请求参数,用户可以自由选择                 |
| UseLog4net                | 否     | 使用log4net记录日志，可选配置，如果这项被起用，那么该组件就会记录一些关键的log日志，例如：重试、异常、链接关闭等             |  
``` C#  
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
```
* **步骤二：创建消费者业务处理类(以routing模式为例)**  
``` C#
namespace Stack.RabbitMQ.ServiceTest.Consumers
{
    /// <summary>
    /// Direct消费者测试类
    /// </summary>
    public class RoutingHandler : IConsumer
    {
        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ResponseResult Handler(ConsumerContext context)
        {
            var number = 0;
            var number1 = 1 / number;
            return new ResponseResult()
            {
                Success = true,
                Data = "Direct测试数据返回结果！！！"
            };
        }
    }
}
```  
* **添加节点配置并重启消费者服务(在rabbitmq.json文件的Services节点下面添加如下配置)** 
``` C#
{
      "Durable": true,
      "IsAudit": true,
      "PatternType": "Routing",
      "RetryTimeRules": [ 1, 30, 10 ],
      "QueueName": "queue.direct.routinghandler",
      "AssemblyName": "Stack.RabbitMQ.ServiceTest",
      "NameSpace": "Stack.RabbitMQ.ServiceTest.Consumers",
      "ClassName": "DirecttHandler"
    }
```
添加玩节点并重新启动一下服务，则会出现如下的队列，那么就说明消费者运行成功,这个时候也就说明消费者端就已经完成，下面我们就可以看看客户端怎么去调用了    ![消费者节点](https://github-1251498502.cos.ap-chongqing.myqcloud.com/RabbitMQ/20180904202328.png)  


## Stack.RabbitMQ 生产者者（客户端）    
#### 生产者客户端配置介绍  

|          配置名称         |是否必填|                            配置说明                                                                              |
|---------------------------|--------|------------------------------------------------------------------------------------------------------------------|
| ConnectionString.Host     | 是     | 业务主机地址                                                                                                     |
| ConnectionString.Port     | 是     | 业务端口                                                                                                         |
| ConnectionString.TimeOut  | 是     | 通道链接超时时间（单位：秒）                                                                                     |
| ConnectionString.UserName | 是     | rabbitmq账户                                                                                                     |
| ConnectionString.Password | 是     | rabbitmq密码                                                                                                     |  
```javascript

{
  "ConnectionString": {
    "Host": "192.168.3.10",
    "Port": 5672,
    "TimeOut": 30,
    "UserName": "admin",
    "Password": "ChinaNet910111"
  }
}
```
好啦，配置添加完毕，下面我们就来导入配置（该步骤与服务端类似，TestHostService标识我们的测试主机，因为是控制台所以用他来做测试）  
``` C#
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
```  


