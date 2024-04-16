﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://localhost:5672");
        using (var connection = factory.CreateConnection())
        {
            var channel = connection.CreateModel();
            //durable=true keeps your query even comp restarts
            //exclusive=true only allow requests from the same channel
            //autodelete=true - if subscriber is down, queue will be deleted
            channel.QueueDeclare("hello-queue", true, false, false);//if publisher haven't created the queue, you can create here
            //global:true tek seferde verecegi sayi, butun subcscriberler icin toplam sayi, false yaparsan subscriber basina o kadar verir
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            //autoAck will remove the message from the queue
            channel.BasicConsume("hello-queue", false, consumer);
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1000);
                Console.WriteLine("Message: "+ message);
                channel.BasicAck(e.DeliveryTag,false);
              //  Console.ReadLine();
            };

            Console.ReadLine();

        }
    }

    
}