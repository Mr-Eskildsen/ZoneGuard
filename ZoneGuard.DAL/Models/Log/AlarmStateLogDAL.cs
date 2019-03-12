
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZoneGuard.DAL.Models.Config;

namespace ZoneGuard.DAL.Models.Log
{
    [Table("log_AlarmState")]
    public class AlarmStateLogDAL 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Triggered { get; set; }

        public int AlarmZoneId { get; set; }

        [ForeignKey("AlarmZoneId")]       
        public AlarmZoneDAL AlarmZone { get; set; }

        [Required]
        public DateTime CreatedTimestamp { get; set; }
        public DateTime EditTimestamp { get; set; }
        
        public String AcknowledgedDescription { get; set; }
        public Nullable<DateTime> AcknowledgedTimestamp { get; set; }
    }
}
