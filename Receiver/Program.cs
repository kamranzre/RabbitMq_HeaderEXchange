using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // یک تفاوت داردheader در گیرنده 
            // دیگر نیز داردProperty که یک 
            //x-matchبه نام 
            //all,anyدو ورودی میگیرد یکی 
            // باشد به این معناست که باید تمامی شروط برقرار باشدall اگر 
            //مثال
            // که ایجاد کرده ای م دو شرط دارد header بر اساس 
            //باشدsubject :personیکی اینکه باید 
            // باشدaction : create و دیگری 
            // x-match بر این اساس اگر
            // باشد پس باید همه شرط ها برقرار باشدall بر روی
            // باشد یکی از شروط هم برقرار باشد کافی استany ولی اگر 
            var headers = new Dictionary<string, object>()
            {
                {"subject","person" },
                {"action","edit" },
                {"x-match","any" }
            };

            channel.QueueDeclare("edit", false, true);
            //نداریمroutingKey نیازی به Exchange در زمان بایند کردن صف به 
            //  را پاس دهیمHeader میگیرد که در اینجا میتوانیم arguments و در ورودی بعدی از ما یک 
            channel.QueueBind("edit", "headersSender", "",arguments:headers);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArg) =>
            {
                var MsgArray = eventArg.Body.ToArray();
                var body = Encoding.UTF8.GetString(MsgArray);
                // eventArgدر Header راه برای بدست آوردن 
                var subject = Encoding.UTF8.GetString(eventArg.BasicProperties.Headers["subject"] as byte[]);
                var action = Encoding.UTF8.GetString(eventArg.BasicProperties.Headers["action"] as byte[]);

                Console.WriteLine($"HeaderCreate:{subject}");
                Console.WriteLine($"HeaderCreate:{action}");
                Console.WriteLine($"ReceiverEdit{body}");
                channel.BasicAck(eventArg.DeliveryTag, true);
            };
            channel.BasicConsume("edit", false, consumer);
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
