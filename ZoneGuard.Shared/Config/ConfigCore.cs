using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace ZoneGuard.Shared.Config
{
        public enum ConfigClassType { Unknown, ServiceMQ, ServiceMQTT };

        public abstract class ConfigCore
        {
            //public const string PARAMETER_ID = "id";
            public const string PARAMETER_NAME = "name";
            public const string PARAMETER_TYPE = "type";
            public const string PARAMETER_CONFIG_CLASS = "config_class";
            public const string PARAMETER_THING_CLASS = "thing_class";
            public const string PARAMETER_THING_CATEGORY = "category";
            public const string PARAMETER_NODE_ID = "node_id";
            public const string PARAMETER_TOPIC_STATE = "topic_state";
            public const string PARAMETER_TOPIC_COMMAND = "topic_command";
            public const string PARAMETER_IS_PERIMETER = "is_perimeter";

            [JsonIgnore]
            private Dictionary<string, string> dictParameters = null;

            [JsonProperty(PropertyName = PARAMETER_NAME)]
            public string Id { get { return getParameterValue(PARAMETER_NAME); } }

            [JsonProperty(PropertyName = PARAMETER_THING_CLASS)]
            public string ThingClass { get { return getParameterValue(PARAMETER_THING_CLASS); } }

            [JsonProperty(PropertyName = PARAMETER_CONFIG_CLASS)]
            public string ConfigClass { get { return getParameterValue(PARAMETER_CONFIG_CLASS); } }


            [JsonProperty(PropertyName = PARAMETER_THING_CATEGORY)]
            public string TypeCategory { get { return getParameterValue(PARAMETER_THING_CATEGORY); } }
            /*
            [JsonProperty(PropertyName = "parameters")]
            public Dictionary<string, string> Parameters
            {
                get
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("topic", "test!");
                    return dict;
                }
            }
            */


            public ConfigCore(Dictionary<string, string> parameters)
            {

                dictParameters = new Dictionary<string, string>();

                if (parameters != null)
                {
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        addParameter(entry.Key, entry.Value);
                    }
                }

                ValidateParameters(parameters);
            }


            protected bool HasParameter(string name)
            {
                return dictParameters.ContainsKey(name);
            }

            protected void addParameter(string name, string value)
            {
                dictParameters.Add(name, value);
            }

            public string getParameterValue(string name)
            {
                if (!dictParameters.ContainsKey(name))
                    return "";

                return dictParameters[name];
            }


            public static ConfigCore CreateFromJSON(String json)
            {
                String strClassPath = "HomeManager2.Shared.Config.";
                String strClassName = "";
                ConfigCore config = null;
                Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);


                strClassName = parameters[PARAMETER_CONFIG_CLASS];
                /*
                //String name = myType.FullName;
                switch (parameters[PARAMETER_CONFIG_TYPE])
                {
                    case "action":
                        strClassPath = Assembly.GetExecutingAssembly().FullName + ".Thing.Action.";
                        break;
                    case "link":
                        strClassPath = Assembly.GetExecutingAssembly().FullName + ".Thing.Link.";
                        break;
                    case "sensor":
                        strClassPath = Assembly.GetExecutingAssembly().FullName + ".Thing.Sensor.";
                        break;
                    case "service":
                        //strClassPath = Assembly.GetExecutingAssembly().FullName + ".Thing.Service.";
                        strClassPath = "HomeManager2.Shared.Thing.Service.";
                        strClassPath = "HomeManager2.Shared.Config.";
                        break;
                    default:
                        throw new NotSupportedException();
                }
                */

                Type clsType = Assembly.GetExecutingAssembly().GetType(strClassPath + strClassName);
                config = (ConfigCore)Activator.CreateInstance(clsType, parameters);


                return config;
            }


            protected static T NA_CreateFromJSON<T>(String json) where T : ConfigCore, new()
                //protected static T CreateFromJSON<T>(String json => new T(json));
            {
                T obj = default(T);
                //T obj = new T();
                Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                /*
                String name = myType.FullName;
                ServiceCore serviceObj = null; // new ServiceMQ();
                                               //myObject = (MyAbstractClass)Activator.CreateInstance("AssemblyName", "TypeName");
                Type cls = Assembly.GetExecutingAssembly().GetType("HomeManager2.Shared.Thing.Service." + clsName);
                // serviceObj = Activator.CreateInstance(cls);




                string name = "MyNamespace.Customer";

                Type targetType = Type.GetType(name);

                Type genericType = typeof(GenericRepository<>).MakeGenericType(targetType);

                object instance = Activator.CreateInstance(genericType);



                T obj = new T();
                Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                foreach (KeyValuePair<string, string> param in parameters)
                {
                    obj.addParameter(param.Key, param.Value);
                }*/
                return obj;
            }

            //protected abstract void onParameterFromJSON(string name, string value);

            //public abstract string toJSON();
            public string toJSON()
            {
                return JsonConvert.SerializeObject(this);
            }


            public static Assembly GetConfigAssembly()
            {
                return Assembly.GetExecutingAssembly();
            }



            protected virtual bool ValidateParameters(Dictionary<string, string> parameters)
            {
                //TODO:: Validate Paarmeter list
                return true;
            }


        }
    }
