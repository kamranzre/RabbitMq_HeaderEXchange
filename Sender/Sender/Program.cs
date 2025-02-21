using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            //اگر قصد داشته باشیم که توسط ExchangeHeader 
            //اطلاعات ارسال کنیم 
            // قرار دهیم Header که ایجاد کرده ایم را Exchange ابتدا باید تایپ

            channel.ExchangeDeclare("headersSender", ExchangeType.Headers);
            //  ایجاد کرد مانند زیرDictionaryایجاد میکنیم میتوان آن را از نوع Header سپس یک 
            var headers = new Dictionary<string, object>()
            {
                {"subject","person" },
                {"action","create" }
            };
            string Message = $"Data Exchange Topic At {DateTime.Now.Second}";
            var body = Encoding.UTF8.GetBytes(Message);

            // قرار دهیدBasicPublish سپس باید هدر را در ورودی 
            // قرار گیردbasicProperties باید به عنوان یک دیتا در Dictionary 

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = headers;


            //**توجه**//
            // نیست routingKeyدیگر نیازی به  
            channel.BasicPublish("headersSender", routingKey:"", basicProperties, body);
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
