using System;
using ZoneGuard.Shared.Config;
using ZoneGuard.Shared.Interface;


namespace ZoneGuard.Shared.Thing.Sensor
{

    public class SensorTriggeredEventArgs : EventArgs
    {
        public SensorTriggeredEventArgs(string name, bool triggered)
        {
            Name = name;
            Triggered = triggered;
        }
        public string Name { get; }
        public bool Triggered { get; }
    }

    //public event void EventHandler<DownloadCompleteEventArgs> FileDownloaded;

    public abstract class SensorCore : ThingCore
    {
        protected event EventHandler<SensorTriggeredEventArgs> TriggeredChanged;


        public bool Triggered
        {
            get;
            protected set;
        }



        protected SensorCore(ConfigSensor config, IManager manager) : base(config, manager)
        {
            NodeId = config.NodeId;
        }

        public void addLink(EventHandler<SensorTriggeredEventArgs> callback)
        {
            TriggeredChanged += callback;
        }

        protected virtual void RaiseTriggeredChanged(SensorTriggeredEventArgs eventArgs)
        {
            TriggeredChanged?.Invoke(this, eventArgs);
        }

        public string NodeId
        {
            get;
            protected set;
        }

    }
}
