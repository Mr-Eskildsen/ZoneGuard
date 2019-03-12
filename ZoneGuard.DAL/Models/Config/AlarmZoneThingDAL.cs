using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using ZoneGuard.DAL.Models.Config;


namespace ZoneGuard.DAL.Models.Config
{
    [Table("config_AlarmZoneThing")]
    public class AlarmZoneThingDAL
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int Enabled { get; set; }

        public int ThingId { get; set; }

        [ForeignKey("ThingId")]
        public ThingDAL Thing { get; set; }


        public int AlarmZoneId { get; set; }
        [ForeignKey("AlarmZoneId")]
        public AlarmZoneDAL AlarmZone { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
