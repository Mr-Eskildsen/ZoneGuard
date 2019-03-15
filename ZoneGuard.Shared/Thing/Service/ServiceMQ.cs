using System;

using System.Text;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

using RabbitMQ.Client.Events;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;
using ZoneGuard.Shared.Thing.Service;

namespace ZoneGuard.Shared.Thing.Service
{

    public class ServiceMQ : ServiceCore
    {
        ILogger logger1 = null;

        public const string QUEUE_CONTROL_NODE_SERVER = "server";

        public const string EXCHANGE_CONTROL = "zoneguard.control";
        //        public const string EXCHANGE_CONFIG = "homemgr.config";
        public const string EXCHANGE_STATE = "zoneguard.state";

        public const string QUEUE_CONTROL = "zoneguard.control";
        public const string QUEUE_STATE = "zoneguard.state";
//        public const string QUEUE_CONFIG = "homemgr.config";
        

        private IConnection mq_connection;
        private IModel mq_channel;

        public ServiceMQ(ConfigServiceMQ config, IManager manager) : base(config, manager)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole();
                //.AddDebug();
        }


        public override void Dispose()
        {
            if (mq_connection.IsOpen)
            {
                mq_connection.Close();
                
            }
            mq_connection = null;
        }

        protected override void onInitialize()
        {
            Connect();
        }

        protected override void onDestroy()
        {
            throw new NotImplementedException();
        }


        private void Connect()
        { 
            ConfigServiceMQ config = getConfig<ConfigServiceMQ>();
            //throw new NotImplementedException();
            getLogger().LogInformation("Connecting to MQ Service");

            ConnectionFactory factory;
            //factory = new ConnectionFactory() { HostName = config.Host };
            factory = new ConnectionFactory
             {
                 HostName = config.Host,
//               Port = 5672,
                 UserName = config.User,
                 Password = config.Password,
                 VirtualHost = config.VirtualHost,
                 RequestedHeartbeat = 60
             };



//            factory.UserName = config.User;
  //          factory.Password = config.Password;
            //factory.VirtualHost = "/";// config.VirtualHost;
            //factory.Port = 5672;

            //TODO:: Handle Server not found (Timeout)
            mq_connection = factory.CreateConnection( );
            //using (mq_connection = factory.CreateConnection())
            {
                mq_channel = mq_connection.CreateModel();

                //Create Configuration exchange
                //mq_channel.ExchangeDeclare(EXCHANGE_CONFIG, "fanout");
                

                //Create Control exchange
                mq_channel.ExchangeDeclare(EXCHANGE_CONTROL, "direct");

                //Create State exchange
                mq_channel.ExchangeDeclare(EXCHANGE_STATE, "fanout");

                /*                using (mq_channel = mq_connection.CreateModel())
                                {
                                    //Create Configuration exchange
                                    mq_channel.ExchangeDeclare("homemgr.config", "fanout");

                                    //Create State exchange
                                    mq_channel.ExchangeDeclare("homemgr.state", "fanout");

                                }
                                */
            }
        }

        public void Publish(string exchangeName, string routingKey, string message)
        {
            //var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            mq_channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }

        public void CreateQueue(EventHandler<BasicDeliverEventArgs> callback)
        {
            CreateQueue(EXCHANGE_STATE, QUEUE_STATE, callback);
        }
        /*
        public void CreateConfigQueue(string nodeId, EventHandler<BasicDeliverEventArgs> callback)
        {
            string queueName = QUEUE_CONFIG + "." + nodeId;
            CreateQueue(EXCHANGE_CONFIG, queueName, callback, nodeId);
        }
        */

        public void CreateStateQueue(EventHandler<BasicDeliverEventArgs> callback)
        {
            string queueName = QUEUE_STATE;
            CreateQueue(EXCHANGE_STATE, queueName, callback);
        }

        public void CreateServerControlQueue(EventHandler<BasicDeliverEventArgs> callback)
        {
            string queueName = QUEUE_CONTROL + "." + QUEUE_CONTROL_NODE_SERVER;
            CreateQueue(EXCHANGE_CONTROL, queueName, callback, QUEUE_CONTROL_NODE_SERVER, false, true, false);
        }


        public void CreateControlQueue(string nodeId, EventHandler<BasicDeliverEventArgs> callback)
        {
            string queueName = QUEUE_CONTROL + "." + nodeId;
            CreateQueue(EXCHANGE_CONTROL, queueName, callback, nodeId);
        }


        protected void CreateQueue(string exchangeName, string queueName, EventHandler<BasicDeliverEventArgs> callback, string routingKey = "", bool autoDelete = true, bool durable = false, bool exclusive = false)
        {
            mq_channel.QueueDeclare(queue: queueName,
                                    durable: durable,
                                    exclusive: exclusive,
                                    autoDelete: autoDelete,
                                    arguments: null);

            mq_channel.QueueBind(queue: queueName,
                             exchange: exchangeName,
                             routingKey: routingKey);

            var consumer = new EventingBasicConsumer(mq_channel);
            
            consumer.Received += callback; /*(model, ea) =>
            {HEST
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] {0}", message);

                
                                paramSensor = new Dictionary<string, string>
                            {
                                {"category", "sensor" },
                                { "config_class", "ConfigSensor" },
                                { "thing_class", "SensorMQTT" },
                                { "id", "LivingroomWindow1" },
                                { "topic", "test/livingroomwindow1/state" }
                            };
                                configSensor = new ConfigSensor(paramSensor);
                                addSensor(new SensorMQTT(configSensor, this));
                
            };
            */
            mq_channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: consumer);
            
        }



        /*

        static void mq_fanout()
        {

            //TODO:: HArdcoded
            var factory = new ConnectionFactory() { HostName = "192.168.1.120" };
            factory.UserName = "svc-homemanager";
            factory.Password = "HomeManager01";
            factory.VirtualHost = "/";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("hello-exchange", "fanout");

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                             routingKey: "hello",
                                             basicProperties: null,
                                             body: body);


                Console.WriteLine(" [x] Sent {0}", message);
            }
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
        */
        /*
        public static void test()
        {
            var factory = new ConnectionFactory() { HostName = "192.168.1.120" };
            factory.UserName = "svc-security";
            factory.Password = "P@ssw0rd";
            factory.VirtualHost = "/";

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: true,
                                     arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                             routingKey: "hello",
                                             basicProperties: null,
                                             body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        */
    }
}
