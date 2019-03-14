using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZoneGuard.DAL.Models.Log
{
    [Table("log_SensorState")]
    public class SensorStateLogDAL
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Column(Order = 1)]
        public int Id { get; set; }

        [Required, StringLength(50), Column(Order = 2)]
        public String SensorName { get; set; }

        [Required, Column(Order = 3)]
        public int Triggered { get; set; }

        
        //public bool TriggeredBool { get; set; }

        [Required, Column(Order = 4)]
        public DateTime Timestamp { get; set; }

    }
}
