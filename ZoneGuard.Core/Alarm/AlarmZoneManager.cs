using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZoneGuard.Core.Thing.Alarm;

namespace ZoneGuard.Core.Alarm
{
    public abstract class AlarmZoneManager
    {
        //private AlarmManagerContext dbContext;
        

        private Dictionary<string, AlarmZone> dictAlarmZones = new Dictionary<string, AlarmZone>();

        private bool _Armed;
        public bool Armed {
            get {
                return _Armed;
            }
            set {
                if (CanChangeArmedState(_Armed, value)) { 
                    this._Armed = value;
                }
            }
        }
        public uint Sensitivity { get; protected set; }



        public AlarmZoneManager()
        {
            //AlarmManagerContextFactory factory = new AlarmManagerContextFactory();
            //dbContext = factory.CreateDbContext();
        }

        public void addAlarmZone(AlarmZone alarmZone)
        {
            dictAlarmZones.Add(alarmZone.Id, alarmZone);
        }

        private bool CanChangeArmedState(bool currentValue, bool requestedValue)
        {
            bool success = false;
            bool newState = false;
            


            //Request for change of state
            if (requestedValue != currentValue)
            {
                BeforeChangeArmedState(currentValue, requestedValue);

                if (requestedValue)
                {
                    if (CanArm())
                    {
                        success = true;
                        ArmZones();
                        newState = requestedValue;
                    }
                    else
                    {
                        success = false;
                        newState = currentValue;
                    }
                }
                else
                {
                    if (CanDisarm())
                    {
                        success = true;
                        DisarmZones();
                        newState = requestedValue;
                    }
                    else
                    {
                        success = false;
                        newState = requestedValue;
                    }
                }
                //Wait a second
                Thread.Sleep(1000);
            }
            else
            {
                newState = requestedValue;
            }
            AfterChangeArmedState(newState);
            return success;
        }

        public virtual void BeforeChangeArmedState(bool currentValue, bool newValue) { }
        public virtual void AfterChangeArmedState(bool value) { }

        protected void ArmZones()
        {
            foreach (AlarmZone alarmZone in dictAlarmZones.Values)
            {
                alarmZone.Armed = true;
            }
        }

        protected void DisarmZones()
        {
            foreach (AlarmZone alarmZone in dictAlarmZones.Values)
            {
                alarmZone.Armed = false;
            }

        }


        protected bool CanArm()
        {
            bool result = true;
            List<string> messages = new List<string>();
            foreach (AlarmZone alarmZone in dictAlarmZones.Values)
            {
                result &= alarmZone.CanArm();
            }
            return true;
        }

        protected bool CanDisarm()
        {
            bool result = true;
            List<string> messages = new List<string>();
            foreach (AlarmZone alarmZone in dictAlarmZones.Values)
            {
                result &= alarmZone.CanDisarm();
            }
            return result;
        }

    }



}
