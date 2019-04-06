using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MQ_Demo
{
    /// <summary>
    /// 1、 简单消息队列
    /// 默认交换隐式绑定到每个队列，路由密钥等于队列名称。它无法显式绑定到默认交换或从默认交换中取消绑定。它也无法删除。
    /// </summary>
    public class SampleMq
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public static string QueueName { get; set; } = "nee32.sample_queue";

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="type">1生产者 2消费者</param>
        public static void ExcuteHandle(string type = "1")
        {
            using (IConnection conn = MqFactory.rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //声明一个消息队列 
                    //参数说明：queue: 队列名称。durable：设置是否执行持久化。如果设置为true，即durable = true，持久化实现的重要参数。
                    //exclusive：指示队列是否是排他性。如果一个队列被声明为排他队列，该队列仅对首次申明它的连接可见，并在连接断开时自动删除。需要注意：1.排他队列是基于连接可见的，同一连接的不同信道Channel是可以同时访问同一连接创建的排他队列；2.“首次”，如果一个连接已经声明了一个排他队列，其他连接是不允许建立同名的排他队列的，这个与普通队列不同；3.即使该队列是持久化的，一旦连接关闭或者客户端退出，该排他队列都会被自动删除的，这种队列适用于一个客户端发送读取消息的应用场景。
                    //autoDelete: 是否自动删除。如果该队列没有任何订阅的消费者的话，该队列会被自动删除。这种队列适用于发布订阅方式创建的临时队列。
                    channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    //持久化消息
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    //properties.DeliveryMode = 2;

                    //生产者
                    if (type == "1")
                    {
                        for (int i = 1; i < 10000; i++)
                        {
                            Thread.Sleep(500);
                            var body = Encoding.UTF8.GetBytes("message ：" + i);

                            //默认交换隐式绑定到每个队列，路由密钥等于队列名称。它无法显式绑定到默认交换或从默认交换中取消绑定。它也无法删除。
                            channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: properties, body: body);
                            Console.WriteLine("queue {0}发送：{1}", QueueName, i);
                        }
                        Console.ReadKey();
                    }
                    else
                    {
                        //channel.QueueBind(queue: "myfirst_mq", exchange: "", routingKey: "myfirst_mq");
                        var consumer = new EventingBasicConsumer(channel);
                        Console.WriteLine("队列{0}等待接收消息...", QueueName);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("queue {0}接收：{1} Done...", QueueName, message);
                        };
                        channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
                        Console.ReadLine();
                    }
                }
            }
        }

    }
}
