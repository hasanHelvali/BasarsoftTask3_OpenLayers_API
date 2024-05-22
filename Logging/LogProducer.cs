using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace BasarSoftTask3_API.Logging
{
    public class LogProducer
    {
        string exchangeName = "example-pub-sub-exchange";
        //ConnectionFactory connectionFactory=new ConnectionFactory();
        //public  LogProducer()
        //{
        //    using IConnection connection = connectionFactory.CreateConnection();

        //    using IModel channel = connection.CreateModel();

        //    connectionFactory.Uri = new Uri("amqp://guest:guest@localhost:5672/");

        //    //publish subscribe exchange
        //    string exchangeName = "example-pub-sub-exchange";


        //    channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
        //        byte[] logByteMessage = Encoding.UTF8.GetBytes("Selamlar");

        //        channel.BasicPublish(exchange: exchangeName, routingKey: "", body: logByteMessage);



        //}

        private readonly IConnection _connection;
        private readonly IModel channel;
        public LogProducer()
        {
            var factory = new ConnectionFactory();
            _connection = factory.CreateConnection();
            channel = _connection.CreateModel();
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672/");
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);
        }
        public void SendLog(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: "",
                                 body: body);
        }

    }
}
