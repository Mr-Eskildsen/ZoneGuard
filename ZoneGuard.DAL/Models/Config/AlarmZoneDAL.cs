    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZoneGuard.DAL.Models.Config
{
    [Table("config_AlarmZone")]
    public class AlarmZoneDAL
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public int Enabled { get; set; }


        public ICollection<AlarmZoneThingDAL> Sensors { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
