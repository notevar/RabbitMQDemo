using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ_Demo
{
    public static class MqFactory
    {
        /// <summary>
        /// 连接配置
        /// </summary>
        public static ConnectionFactory rabbitMqFactory { get; private set; }

        static MqFactory()
        {
            rabbitMqFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
            };
            Console.WriteLine("static MqFactory");
        }
    }
}
