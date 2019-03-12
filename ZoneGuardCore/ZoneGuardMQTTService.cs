using System.Collections.Generic;
using ZoneGuard.Shared.Daemon;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using System;
using MQTTnet;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZoneGuard.Shared.Config.Service;

namespace ZoneGuard.Core
{
    class ZoneGuardMQTTService : CoreDaemonService
    {
        private ConfigServiceMQTT configMQTT;

        private IMqttClient mqttClient = null;
        private IMqttClientOptions clientOptions = null;
        private Dictionary<string, CallbackEventHandler> subscriptions;

        public delegate void CallbackEventHandler(string topic, string message, DateTime timestamp);




        public ZoneGuardMQTTService(IConfiguration configuration, IHostingEnvironment environment, ILogger<ZoneGuardMQTTService> logger, IOptions<ConfigServiceMQTT> config, IApplicationLifetime appLifetime)
            : base(configuration, environment, logger, config, appLifetime)
        {
            configMQTT = config.Value;
            /*
            string host = configuration["host"];
            string port = configuration["port"];
            string user = configuration["user"];
            string password = configuration["password"];
            */

            //This returns a correct service, but prevents me from adding additional AbstractBackgroundProcessService implementations with different type parameters.
            //Additionally, it seems like this reference was newly created, and not the instance that was created on application startup (i.e. the hash codes are different, and the constructor is called an additional time).
            //var service = g. GetService<ZoneGuardAlarmManagerService>();
            
            
        }

        protected override void OnStopping()
        {
            Logger.LogDebug("OnStopping method called.");

            // On-stopping code goes here  
        }

        protected override void OnStopped()
        {
            Logger.LogDebug("OnStopped method called.");

            // Post-stopped code goes here  
        }

        protected override void OnInitializing()
        {
            Logger.LogInformation("OnInitializing method called.");


            subscriptions = new Dictionary<string, CallbackEventHandler>();
            MqttServer_Connect();

        }

        protected override void OnInitialized()
        {
            Logger.LogDebug("OnInitialized method called.");


            try
            {
                
                
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();

                lock (mqttClient)
                {
                    if (configMQTT.UseCredentials())
                    {
                        clientOptions = new MqttClientOptionsBuilder()
                                    .WithClientId(Guid.NewGuid().ToString())
                                    .WithTcpServer(configMQTT.Host)
                                    .WithCredentials(configMQTT.UserName, configMQTT.Password)
                                    .Build();

                    }
                    else
                    {
                        clientOptions = new MqttClientOptionsBuilder()
                                    .WithClientId(Guid.NewGuid().ToString())
                                    .WithTcpServer(configMQTT.Host)
                                    .Build();

                    }

                    //Set delegate for processing MQTT messages
                    mqttClient.ApplicationMessageReceived += onMqttMessageReceived;


                    mqttClient.Connected += async (s, e) =>
                    {
                        string msg = "### MQTT Service CONNECTED WITH MQTT-SERVER ###";
                        Logger.LogDebug(msg);
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

                        mqttClient.ConnectAsync(clientOptions);
                        while (!mqttClient.IsConnected)
                            Thread.Sleep(100);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                    }
                }

                Logger.LogDebug("### WAITING FOR APPLICATION MESSAGES ###");

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            

        }




        public void Publish(string topic, string payload)
        {
            
            MqttApplicationMessage msg = new MqttApplicationMessage();
            msg.Topic = topic;
            //msg.Payload = Encoding.ASCII.GetBytes(payload);
            msg.Payload = Encoding.UTF8.GetBytes(payload);
            mqttClient.PublishAsync(msg);
        }

        public void onMqttMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Logger.LogDebug("### RECEIVED APPLICATION MESSAGE (EVENTHANDLER) ###");

            //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");

            //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            //Console.WriteLine();

            if (subscriptions.ContainsKey(e.ApplicationMessage.Topic))
            {
                string topic = e.ApplicationMessage.Topic;
                string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                //string topic, string message, DateTime timestamp
                subscriptions[e.ApplicationMessage.Topic](topic, message, DateTime.UtcNow);
            }
            else
            {
                throw new NotSupportedException("No Topic registered for message");
            }
            Console.WriteLine();

        }

        public void Test(string topic)
        {
            Logger.LogDebug("### CALLING TEST ###");
        }

        public void Subscribe(string topic, CallbackEventHandler callback)
        {
            lock (mqttClient)
            {
                Logger.LogDebug("### SUBSCRIBING topic='" + topic + "' ###");
                mqttClient.SubscribeAsync(new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce));
                if (subscriptions.ContainsKey(topic))
                {
                    subscriptions.Remove(topic);
                }
                subscriptions.Add(topic, callback);

                Console.WriteLine("### SUBSCRIBED topic='" + topic + "' ###");
            }
        }

        private void MqttServer_Connect()
        {
            try
            {
                /*
                ConfigServiceMQTT config = getConfig<ConfigServiceMQTT>();
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();

                lock (mqttClient)
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

                        mqttClient.ConnectAsync(clientOptions);
                        while (!mqttClient.IsConnected)
                            Thread.Sleep(100);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                    }
                }
                */
                Logger.LogDebug("### WAITING FOR APPLICATION MESSAGES ###");

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }




#if MQTT_MOVED
public ServiceMQTT(ConfigServiceMQTT config, IManager manager) : base(config, manager)
        {

        }

        protected override void onInitialize()
        {
            subscriptions = new Dictionary<string, CallbackEventHandler>();

            MqttServer_Connect();
            //InitAsync();
            //DoItAsync();

            //Task.Run(DoItAsync);

            //Task.Run(RunAsync);
        }


        public void Publish(string topic, string payload)
        {
            MqttApplicationMessage msg = new MqttApplicationMessage();
            msg.Topic = topic;
            //msg.Payload = Encoding.ASCII.GetBytes(payload);
            msg.Payload = Encoding.UTF8.GetBytes(payload);
            mqttClient.PublishAsync(msg);
        }

        public void onMqttMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            getLogger().LogDebug("### RECEIVED APPLICATION MESSAGE (EVENTHANDLER) ###");

            //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");

            //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            //Console.WriteLine();

            if (subscriptions.ContainsKey(e.ApplicationMessage.Topic))
            {
                string topic = e.ApplicationMessage.Topic;
                string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

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
            lock (mqttClient)
            {
                getLogger().LogDebug("### SUBSCRIBING topic='" + topic + "' ###");
                mqttClient.SubscribeAsync(new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce));
                if (subscriptions.ContainsKey(topic))
                {
                    subscriptions.Remove(topic);
                }
                subscriptions.Add(topic, callback);

                Console.WriteLine("### SUBSCRIBED topic='" + topic + "' ###");
            }
        }

        private void MqttServer_Connect()
        {
            try
            {

                ConfigServiceMQTT config = getConfig<ConfigServiceMQTT>();
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();

                lock (mqttClient)
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

                        mqttClient.ConnectAsync(clientOptions);
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
#endif





#if USE_MQTT








        




        

        public void Unsubscribe(string topic)
        {
            lock (mqttClient)
            {
                subscriptions.Remove(topic);
                mqttClient.UnsubscribeAsync(new List<string> { topic });
                getLogger().LogDebug("### UNSUBSCRIBED topic='" + topic + "' ###");
            }
        }


        /*
        private async Task InitAsync()
        {
            try {

                ConfigServiceMQTT config = getConfig<ConfigServiceMQTT>();
                var factory = new MqttFactory();
                mqttClient = factory.CreateMqttClient();


                clientOptions = new MqttClientOptionsBuilder()
                                .WithClientId(Guid.NewGuid().ToString())
                                .WithTcpServer(config.Host)
                                .Build();



                mqttClient.ApplicationMessageReceived += (s, e) =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();
                };

                mqttClient.Connected += async (s, e) =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

//                    await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());

//                    Console.WriteLine("### SUBSCRIBED ###");
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
                }
                catch (Exception exception)
                {
                    Console.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                }

                Console.WriteLine("### WAITING FOR APPLICATION MESSAGES ###");

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }

        */
        /*
        private async Task DoItAsync()
        {
            try {
                bool isSubscribed = false;
                while (true)
                {
                    Console.ReadLine();

                    if (isSubscribed)
                    {
                        var applicationMessage = new MqttApplicationMessageBuilder()
                                                    .WithTopic("test/mysensor2")
                                                    .WithPayload("Goodbye World")
                                                    .WithAtLeastOnceQoS()
                                                    .Build();

                        await mqttClient.PublishAsync(applicationMessage);

                        
                        await Unsubscribe("test/mysensor2");

                        applicationMessage = new MqttApplicationMessageBuilder()
                                                    .WithTopic("test/mysensor2")
                                                    .WithPayload("I am dead")
                                                    .WithAtLeastOnceQoS()
                                                    .Build();

                        await mqttClient.PublishAsync(applicationMessage);


                        isSubscribed = false;
                    }
                    else
                    {
                        await Subscribe("test/mysensor2", this.MyCallback);
                        
                        var applicationMessage = new MqttApplicationMessageBuilder()
                            .WithTopic("test/mysensor2")
                            .WithPayload("Hello World")
                            .WithAtLeastOnceQoS()
                            .Build();

                        await mqttClient.PublishAsync(applicationMessage);

                        applicationMessage = new MqttApplicationMessageBuilder()
                                                    .WithTopic("test/mysensor2")
                                                    .WithPayload("I am alive :-)")
                                                    .WithAtLeastOnceQoS()
                                                    .Build();

                        await mqttClient.PublishAsync(applicationMessage);

                        isSubscribed = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }
        */
        /*
        public async Task RunAsync()
        {
            throw new NotImplementedException();
            try
            {
                //MqttNetConsoleLogger.ForwardToConsole();

                var factory = new MqttFactory();
                var client = factory.CreateMqttClient();
                var clientOptions = new MqttClientOptions
                {
                    ChannelOptions = new MqttClientTcpOptions
                    {
                        Server = "192.168.1.50"
                    }
                };

                client.ApplicationMessageReceived += (s, e) =>
                {
                    Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    Console.WriteLine();
                };

                client.Connected += async (s, e) =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test/mysensor1").Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                };

                client.Disconnected += async (s, e) =>
                {
                    Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    try
                    {
                        await client.ConnectAsync(clientOptions);
                    }
                    catch
                    {
                        Console.WriteLine("### RECONNECTING FAILED ###");
                    }
                };

                try
                {
                    await client.ConnectAsync(clientOptions);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("### CONNECTING FAILED ###" + Environment.NewLine + exception);
                }

                Console.WriteLine("### WAITING FOR APPLICATION MESSAGES ###");
                bool isSubscribed = false;
                while (true)
                {
                    Console.ReadLine();

                    if (isSubscribed)
                    {
                        var applicationMessage = new MqttApplicationMessageBuilder()
                                                    .WithTopic("test/mysensor2")
                                                    .WithPayload("Goodbye World")
                                                    .WithAtLeastOnceQoS()
                                                    .Build();

                        await client.PublishAsync(applicationMessage);

                        List<string> topics = new List<string> { "test/mysensor2" };
                        await client.UnsubscribeAsync (topics);

                        applicationMessage = new MqttApplicationMessageBuilder()
                                                    .WithTopic("test/mysensor2")
                                                    .WithPayload("I am dead")
                                                    .WithAtLeastOnceQoS()
                                                    .Build();

                        await client.PublishAsync(applicationMessage);


                        isSubscribed = false;
                    }
                    else
                    {
                        await client.SubscribeAsync(new TopicFilter("test/mysensor2", MqttQualityOfServiceLevel.AtMostOnce));
                        var applicationMessage = new MqttApplicationMessageBuilder()
                            .WithTopic("test/mysensor2")
                            .WithPayload("Hello World")
                            .WithAtLeastOnceQoS()
                            .Build();

                        await client.PublishAsync(applicationMessage);

                        applicationMessage = new MqttApplicationMessageBuilder()
                                                    .WithTopic("test/mysensor2")
                                                    .WithPayload("I am alive :-)")
                                                    .WithAtLeastOnceQoS()
                                                    .Build();

                        await client.PublishAsync(applicationMessage);

                        isSubscribed = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

*/

        protected void onInitialize_NA()
        {
            ConfigServiceMQTT config = getConfig<ConfigServiceMQTT>();
            //IMqttClientFactory factory = new MqttFactory();
            //IMqttClient mqtt_client = factoryCreateMqttClient();
            var factory = new MqttFactory();
            var mqtt_client = factory.CreateManagedMqttClient();

            /*var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer(config.Host)
                            .Build();
                            */


            var options = new ManagedMqttClientOptions
            {
                ClientOptions = new MqttClientOptions
                {
                    ClientId = Guid.NewGuid().ToString(),
                    //Credentials = new RandomPassword(),
                    ChannelOptions = new MqttClientTcpOptions
                    {
                        Server = "broker.hivemq.com"
                    }
                },

                AutoReconnectDelay = TimeSpan.FromSeconds(1)
            };


            mqtt_client.Connected += async (s, e) =>
            {
                await mqtt_client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test/mysensor1").Build());
            };


            Thread.Sleep(15000);
            bool b = mqtt_client.IsConnected;

            mqtt_client.ApplicationMessageReceived += (sender, e) =>
            {
                Console.WriteLine(sender.ToString());
                Console.WriteLine(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
            };

            mqtt_client.StartAsync(options);

            Thread.Sleep(15000);
            // mysensor1

            mqtt_client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test/mysensor2").Build());

            Thread.Sleep(15000);
            // mysensor1 + mysensor2


            List<string> topics1 = new List<string>(new string[] { "test/mysensor1" });
            mqtt_client.UnsubscribeAsync(topics1);
            Thread.Sleep(15000);

            // mysensor2

            List<string> topics2 = new List<string>(new string[] { "test/mysensor2" });
            mqtt_client.UnsubscribeAsync(topics2);
            Thread.Sleep(15000);

            // None

            mqttClient = (MqttClient)mqtt_client;

            //throw new NotImplementedException();
        }

        protected override void onDestroy()
        {
            throw new NotImplementedException();
        }

        /*
        public void test()
        {




            // register to message received
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            // subscribe to the topic "/home/temperature" with QoS 2
            client.Subscribe(
                new string[] { "testTopic" },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE
        });

        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // handle message received
            Console.WriteLine("message=" + e.Message.ToString());

        }
        */
#endif 

    }
}
