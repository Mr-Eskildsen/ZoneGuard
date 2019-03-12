using ZoneGuard.DAL.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZoneGuard.DAL.Models.State
{

    public class AlarmStateDAL
    {
        public int Id { get; set; }
        public int Triggered { get; set; }
        public DateTime Timestamp { get; set; }

        public int  AlarmZoneId { get; set; }
//        public AlarmZoneConfig AlarmZone { get; set; }
    }
}
