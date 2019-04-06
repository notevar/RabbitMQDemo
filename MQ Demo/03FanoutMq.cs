using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MQ_Demo
{
    /// <summary>
    ///3、 广播式交换机（Fanout Exchange）
    ///exchange匹配 绑定到交换机的队列
    /// </summary>
    public class FanoutMq
    {
        /// <summary>
        /// 路由名称
        /// </summary>
        public static string ExchangeName { get; set; } = "nee32.fanout_exchange";

        /// <summary>
        /// 队列名称
        /// </summary>
        public static string QueueName { get; set; } = "nee32.fanout_queue";

        /// <summary>
        /// 路由键
        /// </summary>
        public static string RoutingKey { get; set; } = "nee32.fanout_routingkey";

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
                    //声明一个消息队列   durable：true 持久化队列
                    //channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    ////持久化消息队列
                    //var properties = channel.CreateBasicProperties();
                    //properties.Persistent = true;
                    //properties.DeliveryMode = 2;

                    //声明一个交换机
                    channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: false);

                    //生产者
                    if (type == "1")
                    {
                        for (int i = 1; i < 1000; i++)
                        {
                            Thread.Sleep(500);
                            var body = Encoding.UTF8.GetBytes("message ：" + i);

                            channel.BasicPublish(exchange: ExchangeName, routingKey: "", basicProperties: null, body: body);
                            Console.WriteLine("exchange {0}发送：{1}", ExchangeName, i);
                        }
                        Console.ReadKey();
                    }
                    else
                    {
                        var queueName = channel.QueueDeclare().QueueName;

                        //绑定交换机到队列
                        channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: "");
                        Console.WriteLine("队列{0}等待接收消息...", queueName);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("queue {0}接收：{1} Done...", QueueName, message);
                        };
                        //关闭消息自动确认autoAck：false
                        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                        Console.ReadLine();
                    }
                }
            }
        }
    }
}
