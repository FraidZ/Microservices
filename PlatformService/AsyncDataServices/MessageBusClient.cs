using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            
            var factory = new ConnectionFactory
                { HostName = _configuration["RabbitMQHost"], 
                     Port = int.Parse(_configuration["RabbitMQPort"]) };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _channel.ExchangeDeclare(exchange:"trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

                Console.WriteLine("--> Connected to message bus");
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {e.Message}");
            }
        }
        
        public void PublishNewPlatform(PlatformPublishedDto dto)
        {
            var message = JsonSerializer.Serialize(dto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection Open. Sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection closed. Not sending");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (!_channel.IsOpen) return;
            
            _channel.Close();
            _connection.Close();
        }
        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel
                .BasicPublish(exchange:"trigger", routingKey:"", basicProperties:null, body: body);

            Console.WriteLine($"We have sent {message}");
        }
        
        private void RabbitMQ_ConnectionShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }
    }
}