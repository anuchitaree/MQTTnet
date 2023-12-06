using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Security.Cryptography.X509Certificates;

namespace PubisherMQTT
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            IMqttClient client = mqttFactory.CreateMqttClient();

            //var tlsOptions = new MqttClientOptionsBuilderTlsParameters
            //{
            //    UseTls = true,
            //    Certificates = new List<X509Certificate> 
            //    {
            //        new X509Certificate("127.0.0.1")
            //    },
            //    AllowUntrustedCertificates = true,
            //    IgnoreCertificateChainErrors = true,
            //    IgnoreCertificateRevocationErrors = true,
            //};

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer("127.0.0.1", 30080)
                //.WithTls(tlsOptions)
                .WithCleanSession()
                .Build();

            client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected to the broker sucessfully");
            });
            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from the broker sucessfully");
            });
            await client.ConnectAsync(options);

            Console.WriteLine("Please press a key to publish the message");

            Console.ReadLine();

            await PublishMessageAsync(client);

            await client.DisconnectAsync();
        }

        private static async Task PublishMessageAsync(IMqttClient client)
        {
            string messagePayload = "Hello";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("temp1")
                .WithPayload(messagePayload)
                .WithAtLeastOnceQoS()
                .Build();
            if (client.IsConnected)
            {
                Console.WriteLine($"Published Message - {messagePayload}");

                await client.PublishAsync(message);

            }
        }

    }
}