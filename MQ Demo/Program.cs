using System;

namespace MQ_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string type = Console.ReadLine();
            //SampleMq.ExcuteHandle(type);
            //WorkMq.ExcuteHandle(type);
            //FanoutMq.ExcuteHandle(type);
            //TopicMq.ExcuteHandle(type);
            HeadersMq.ExcuteHandle(type);
            //WorkMq(type);
            //FanoutMq(type);
            //DirectMq(type);
        }        
    }
}
