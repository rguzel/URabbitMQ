// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var factory = new ConnectionFactory();
        factory.Uri = new Uri("amqp://localhost:5672");
        using (var connection = factory.CreateConnection())
        {
            var channel = connection.CreateModel();
            //durable=true keeps your query even comp restarts
            //exclusive=true only allow requests from the same channel
            //autodelete=true - if subscriber is down, queue will be deleted
            channel.QueueDeclare("hello-queue", true, false, false);

            Enumerable.Range(1, 50).ToList().ForEach(x => {



                string message = $"hello Worldingen  {x}";
                //you can send messages as byte[]
                //so convert your message to byte[]
                var messageBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

                Console.WriteLine($"Message sent! : {message}" );

            });

          
            Console.ReadLine();

        }
    }
}