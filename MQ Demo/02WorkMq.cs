using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MQ_Demo
{
    /// <summary>
    /// 2、 工作队列
    /// </summary>
    public class WorkMq
    {
        /// <summary>
        /// 队列名称
        /// </summary>
        public static string QueueName { get; set; } = "nee32.work_queue";

        /// <summary>
        ///处理消息
        /// </summary>
        /// <param name="type">1生产者 2消费者</param>
        public static void ExcuteHandle(string type = "1")
        {
            using (IConnection conn = MqFactory.rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //声明一个消息队列   durable：true 持久化消息队列
                    channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    //持久化消息
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    //生产者
                    if (type == "1")
                    {
                        for (int i = 1; i < 1000; i++)
                        {
                            Thread.Sleep(500);
                            var body = Encoding.UTF8.GetBytes("message ：" + i);

                            channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: properties, body: body);
                            Console.WriteLine("queue {0}发送：{1}", QueueName, i);
                        }
                        Console.ReadKey();
                    }
                    else
                    {
                        //设置公平派遣  prefetchCount设置预处理条数
                        channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
                        Console.WriteLine("队列{0}等待接收消息...", QueueName);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("queue {0}接收：{1} Done...", QueueName, message);
                            Thread.Sleep(2000);
                            Console.WriteLine("sleep 1500");
                            //消息确认
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        };
                        //关闭消息自动确认autoAck：false
                        channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
                        Console.ReadLine();
                    }
                }
            }
        }
    }
}
