# Stack.RabbitMQ  
### 名词解释 
* **Queue** 队列  
* **Exchange** 交换机  
* **Producer** 生产者  
* **Consumer** 消费者  



### 1. 什么是RabbitMQ？  
RabbitMQ——Rabbit Message Queue的简写，但不能仅仅理解其为消息队列，消息代理更合适。RabbitMQ 是一个由 Erlang 语言开发的AMQP（高级消息队列协议）的开源实现，其内部结构如下：  
![内部结构图](https://github-1251498502.cos.ap-chongqing.myqcloud.com/RabbitMQ/2799767-82c5402158929477_1.png)  

### 2. RabbitMQ能做些什么？  
RabbitMQ作为一个消息代理，主要和消息打交道，负责接收并转发消息。RabbitMQ提供了可靠的消息机制、跟踪机制和灵活的消息路由，支持消息集群和分布式部署。  
适用于**排队算法**、**秒杀活动**、**消息分发**、**异步处理**、**数据同步**、**处理耗时任务**、**CQRS**等诸多应用场景。

#### 简单架构示意图  
RabbitMQ系统最核心的组件是Exchange和Queue，下图是系统简单的示意图。Exchange和Queue是在rabbitmq server（又叫做broker）端，producer和consumer在应用端。
![简单的架构示意图](https://github-1251498502.cos.ap-chongqing.myqcloud.com/RabbitMQ/2799767-82c5402158929477_2.png)  
消费者(consumer)订阅某个队列，生产者(producer)创建消息并通过exchange将消息发布到队列(queue)，最后队列在将消息发送给监听的消费者consumer。  
