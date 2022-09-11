using Newtonsoft.Json;
using RabbitMq.Common;
using RabbitMQ.Client;
using System.Net;
using System.Text;

var random = new Random();

WebClient client = new WebClient();
var downloadedString = client.DownloadString("https://type.fit/api/quotes");

var quoteList = JsonConvert.DeserializeObject<IEnumerable<Quote>>(downloadedString);


var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

    foreach (var quote in quoteList)
    {
        var message = JsonConvert.SerializeObject(quote);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);
        Console.WriteLine(" [x] Sent {0}", message);

        Thread.Sleep(10);
    }

}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
