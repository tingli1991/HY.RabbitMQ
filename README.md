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

### 简单架构示意图  
RabbitMQ系统最核心的组件是Exchange和Queue，下图是系统简单的示意图。Exchange和Queue是在rabbitmq server（又叫做broker）端，producer和consumer在应用端。
![简单的架构示意图](https://github-1251498502.cos.ap-chongqing.myqcloud.com/RabbitMQ/2799767-82c5402158929477_2.png?radom=12122)  
消费者(consumer)订阅某个队列，生产者(producer)创建消息并通过exchange将消息发布到队列(queue)，最后队列在将消息发送给监听的消费者consumer。   

### 队列（Queue）  
消息队列，提供了**先进先出**（FIFO）的处理机制，具有缓存消息的能力。rabbitmq中，队列消息可以设置为持久化，临时或者自动删除。  
* 1、设置为持久化的队列，queue中的消息会在server本地硬盘存储一份，防止系统崩溃，数据丢失  
* 2、设置为临时队列，queue中的数据在系统重启之后就会丢失  
* 3、设置为自动删除的队列，当不存在用户连接到server，队列中的数据会被自动删除  

### 交换机（Exchange）  
RabbitMQ中，producer不是通过信道直接将消息发送给queue，而是先发送给Exchange。**一个Exchange可以和多个Queue进行绑定**，producer在传递消息的时候，会传递一个路由key(ROUTING_KEY)，Exchange会根据这个路由key(ROUTING_KEY)按照特定的路由算法，将消息路由给指定的queue。和Queue一样，Exchange也可设置为持久化，临时或者自动删除。  

#### Exchange的4种类型：    
交换机类型分别有 Direct(默认)、Fanout、 Topic以及headers四种类型，不同类型的Exchange转发消息的策略有所区别：   

* **Direct：** 直接交换器，direct类型的Exchange路由规则也很简单，它会把消息路由到那些binding key与routing key完全匹配的Queue中；  
![直接交换器示意图](https://github-1251498502.cos.ap-chongqing.myqcloud.com/RabbitMQ/2799767-82c5402158929477_5.png)  
当生产者（P）发送消息时Rotuing key=booking时，这时候将消息传送给Exchange，Exchange获取到生产者发送过来消息后，会根据自身的规则进行与匹配相应的Queue，这时发现Queue1和Queue2都符合，就会将消息传送给这两个队列，如果我们以Rotuing key=create和Rotuing key=confirm发送消息时，这时消息只会被推送到Queue2队列中，其他Routing Key的消息将会被丢弃。  

* **Fanout：** 广播是式交换器，fanout类型的Exchange路由规则非常简单，它会把所有发送到该Exchange的消息路由到所有与它绑定的Queue中；  
![直接交换器示意图](https://github-1251498502.cos.ap-chongqing.myqcloud.com/RabbitMQ/2799767-82c5402158929477_4.png)  
上图所示，生产者（P）生产消息1并将消息1推送到Exchange，由于Exchange Type=fanout这时候会遵循fanout的规则将消息推送到所有与它绑定Queue，也就是图上的两个Queue以及最后的两个消费者消费。  

* **Topic：** 主题交换器，前面提到的direct规则是严格意义上的匹配，换言之Routing Key必须与Binding Key相匹配的时候才将消息传送给Queue，那么topic这个规则就是模糊匹配，可以通过通配符满足一部分规则就可以传送；  
**具体的约定如下：**  
1、routing key为一个句点号“. ”分隔的字符串（我们将被句点号“. ”分隔开的每一段独立的字符串称为一个单词），如“stock.usd.nyse”、“nyse.vmw”、“quick.orange.rabbit”；  
2、binding key与routing key一样也是句点号“. ”分隔的字符串；  
3、binding key中可以存在两种特殊字符“*”与“#”，用于做模糊匹配，其中“*”用于匹配一个单词，“#”用于匹配多个单词（可以是零个）；  

 






