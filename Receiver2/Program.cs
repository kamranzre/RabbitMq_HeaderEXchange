using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receiver2
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var headers = new Dictionary<string, object>()
            {
                {"subject","person" },
                {"action","create" },
                {"x-match","all" }
            };

            channel.QueueDeclare("create", false, true);
            channel.QueueBind("create", "headersSender","",headers);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArg) =>
            {
                var MsgArray = eventArg.Body.ToArray();
                var body = Encoding.UTF8.GetString(MsgArray);

               
                Console.WriteLine($"ReceiverCreate{body}");
                channel.BasicAck(eventArg.DeliveryTag,true);
            };
            channel.BasicConsume("create", false,consumer);
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
