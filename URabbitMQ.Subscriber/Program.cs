using RabbitMQ.Client;
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
            string queueName = "GreetingQueue";
            channel.QueueDeclare(queueName, true, false, false);//if publisher haven't created the queue, you can create here
            //prefectSize=0 any size
            //prefectCount how many messages per subscriber/worker
            //global:true total count for all consumers at once if the number =5 it gives 3 for the the first consumer and 2 for the ssecond one
            //if it is false count per consumer, so if the number is 5 it gives 5 for firstconsumer 5 for second consumer
            channel.BasicQos(0,1, false);
            var consumer = new EventingBasicConsumer(channel);
            //autoAck will remove the message from the queue
            channel.BasicConsume(queueName, false, consumer);
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Message: "+ message);
                channel.BasicAck(e.DeliveryTag,false);
              //  Console.ReadLine();
            };

            Console.ReadLine();

        }
    }

    
}