using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZoneGuard.DAL.Models.Config;

namespace ZoneGuard.DAL.Models.Log
{
    [Table("log_SensorState")]
    public class SensorStateLogDAL
    {
        public int Id { get; set; }

        public ThingDAL Sensor { get; set; }

        [Required]
        public int Triggered { get; set; }
        
        public DateTime CreatedTimestamp { get; set; }

    }
}
