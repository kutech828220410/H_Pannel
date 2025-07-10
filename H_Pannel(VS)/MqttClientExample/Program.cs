using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;
using System.Net;

namespace MqttServerExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("啟動 MQTT Server (.NET 5)");

            // 建立 Server 設定
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .WithDefaultEndpointBoundIPAddress(IPAddress.Any)
                .Build();

            var mqttFactory = new MqttFactory();
            var mqttServer = mqttFactory.CreateMqttServer(options);

            mqttServer.ClientConnectedAsync += e =>
            {
                Console.WriteLine($"裝置已連線：ClientId = {e.ClientId}");
                return Task.CompletedTask;
            };

            mqttServer.ClientDisconnectedAsync += e =>
            {
                Console.WriteLine($"裝置已斷線：ClientId = {e.ClientId}");
                return Task.CompletedTask;
            };

            mqttServer.InterceptingPublishAsync += e =>
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"收到訊息 - 主題: {topic}，內容: {payload}");
                return Task.CompletedTask;
            };

            await mqttServer.StartAsync();
            Console.WriteLine("MQTT Server 啟動完成，等待裝置連線中...");

            // 啟動背景任務，每3秒列出目前連線裝置
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    var clients = await mqttServer.GetClientsAsync();
                    Console.WriteLine("=== 目前連線裝置列表 ===");
                    if (clients.Count == 0)
                    {
                        Console.WriteLine("(無連線裝置)");
                    }
                    else
                    {
                        foreach (var client in clients)
                        {
                            Console.WriteLine($"ClientId: {client.Id}");
                        }
                    }
                    Console.WriteLine("========================");
                    await Task.Delay(3000);
                }
            });

            Console.ReadKey();

            await mqttServer.StopAsync();
            Console.WriteLine("MQTT Server 已關閉");
        }
    }
}
