using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZoneGuard.DAL.Models.Config
{

    //public enum SensorType { Door=1, Window=2, Pir=3};
    public enum ThingType { Service = 1, Sensor = 2 };


    [Table("config_Thing")]
    public class ThingDAL
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(NullDisplayText = "Type not set")]
        public ThingType ThingType { get; set; }


        public DateTime Timestamp { get; set; }

        public ICollection<ThingParameterDAL> Parameters { get; set; }


        public int? NodeId { get; set; }
        //public NodeDAL Node { get; set; }

    }
}
