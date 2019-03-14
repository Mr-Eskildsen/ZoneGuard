using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;

namespace ZoneGuard.Shared.Thing.Service
{
    public class ServiceMQTT : ServiceCore
    {

        IMqttClient mqttClient = null;
        IMqttClientOptions clientOptions = null;


        public delegate void CallbackEventHandler(string topic, string message, DateTime timestamp);


        private Dictionary<string, CallbackEventHandler> subscriptions;




        public ServiceMQTT(ConfigServiceMQTT config, IManager manager) : base(config, manager)
        {
            
        }

        protected override void onInitialize()
        {
            subscriptions = new Dictionary<string, CallbackEventHandler>();

            MqttServer_Connect().Wait();

            //InitAsync();
            //DoItAsync();

            //Task.Run(DoItAsync);

            //Task.Run(RunAsync);
        }


        public bool isConnected()
        {
            if (mqttClient!=null)
                return mqttClient.IsConnected;
            return false;
        }

        public void Unsubscribe(string topic)
        {
            UnsubscribeAsync(topic).Wait();
        }

        public async Task UnsubscribeAsync(string topic)
        {
            subscriptions.Remove(topic);
            await mqttClient.UnsubscribeAsync(new List<string> { topic });
            getLogger().LogInformation("### UNSUBSCRIBED topic='" + topic + "' ###");
        }

        public void Publish(string topic, string payload)
        {
            PublishAsync(topic, payload).Wait();
        }

        public async Task PublishAsync(string topic, string payload)
        {
            MqttApplicationMessage msg = new MqttApplicationMessage();
            msg.Topic = topic;
            //msg.Payload = Encoding.ASCII.GetBytes(payload);
            msg.Payload = Encoding.UTF8.GetBytes(payload);
            await mqttClient.PublishAsync(msg);
        }

        public void onMqttMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            getLogger().LogDebug("### RECEIVED APPLICATION MESSAGE (EVENTHANDLER) ###");

            //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            //Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            //Console.WriteLine();

            if (subscriptions.ContainsKey(e.ApplicationMessage.Topic))
            {
                string topic = e.ApplicationMessage.Topic;
                string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                getLogger().LogInformation(String.Format("### RECEIVED APPLICATION MESSAGE Topic='{0}', Payload='{1}' ###",topic, message));

                //string topic, string message, DateTime timestamp
                subscriptions[e.ApplicationMessage.Topic](topic, message, DateTime.UtcNow);
            }
            else
            {
                throw new NotSupportedException("No Topic registered for message");
            }
            Console.WriteLine();

        }
        public void Subscribe(string topic, CallbackEventHandler callback)
        {
            Task t = SubscribeAsync(topic, callback);
            t.Wait();
        }

        public async Task SubscribeAsync(string topic, CallbackEventHandler callback)
        {
            getLogger().LogDebug("### SUBSCRIBING topic='" + topic + "' ###");

            if (subscriptions.ContainsKey(topic))
            {
                Unsubscribe(topic);
                subscriptions.Remove(topic);
            }
            subscriptions.Add(topic, callback);
            await mqttClient.SubscribeAsync( new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce));
            

            getLogger().LogInformation("### SUBSCRIBED topic='" + topic + "'  ###");
        }

        private async Task MqttServer_Connect()
        {
            try
            {

                ConfigServiceMQTT config = getConfig<ConfigServiceMQTT>();
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();
                
                //lock (mqttClient)
                {

                    if (config.UseCrentials())
                    {
                        clientOptions = new MqttClientOptionsBuilder()
                                    .WithClientId(Guid.NewGuid().ToString())
                                    .WithTcpServer(config.Host)
                                    .WithCredentials(config.UserName, config.Password)
                                    .Build();

                    }
                    else
                    {
                        clientOptions = new MqttClientOptionsBuilder()
                                    .WithClientId(Guid.NewGuid().ToString())
                                    .WithTcpServer(config.Host)
                                    .Build();

                    }

                    //Set delegate for processing MQTT messages
                    mqttClient.ApplicationMessageReceived += onMqttMessageReceived;


                    mqttClient.Connected += async (s, e) =>
                    {
                        string msg = "### MQTT Service CONNECTED WITH MQTT-SERVER ###";
                        getLogger().LogDebug(msg);
                        Console.WriteLine(msg);
                    };

                    mqttClient.Disconnected += async (s, e) =>
                    {
                        Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                        await Task.Delay(TimeSpan.FromSeconds(5));

                        try
                        {
                            await mqttClient.ConnectAsync(clientOptions);
                        }
                        catch
                        {
                            Console.WriteLine("### RECONNECTING FAILED ###");
                        }
                    };

                    try
                    {

                        await mqttClient.ConnectAsync(clientOptions);
                        while (!mqttClient.IsConnected)
                            Thread.Sleep(100);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                    }
                }

                getLogger().LogDebug("### WAITING FOR APPLICATION MESSAGES ###");

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }

        protected override void onDestroy()
        {
        }
        
  
    }

}
