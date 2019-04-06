using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace MQ_Demo
{
    /// <summary>
    ///5、 主题式交换机（Topic Exchange）
    ///exchange和RouteKey必须完全匹配
    /// </summary>
    public class TopicMq
    {
        /// <summary>
        /// 路由名称
        /// </summary>
        public static string ExchangeName { get; set; } = "nee32.topic_exchange";

        /// <summary>
        /// 队列名称
        /// </summary>
        public static string QueueName { get; set; } = "nee32.topic_queue";

        /// <summary>
        /// 路由键
        /// </summary>
        public static string RoutingKey { get; set; } = "nee32.topic_routingkey";

        /// <summary>
        ///执行
        /// </summary>
        /// <param name="type">1生产者 2消费者</param>
        public static void ExcuteHandle(string type = "1")
        {
            using (IConnection conn = MqFactory.rabbitMqFactory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //声明一个交换机
                    channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic);
                    //生产者
                    if (type == "1")
                    {
                        for (int i = 1; i < 1000; i++)
                        {
                            Thread.Sleep(500);
                            var body = Encoding.UTF8.GetBytes("message ：" + i);

                            channel.BasicPublish(exchange: ExchangeName, routingKey: RoutingKey, basicProperties: null, body: body);
                            Console.WriteLine("RoutingKey {0}发送：{1}", RoutingKey, i);
                        }
                        Console.ReadKey();
                    }
                    else
                    {
                        var queueName = channel.QueueDeclare().QueueName;

                        //绑定交换机到队列
                        channel.QueueBind(queue: queueName, exchange: ExchangeName, routingKey: "nee32.*");//匹配nee32.xxx  不匹配nee32.xxx.xxx #全匹配

                        Console.WriteLine("等待接收消息...");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("queue {0}接收：{1} Done...", queueName, message);
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
