using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZoneGuard.DAL.Models.Config
{
    [Table("config_ThingParameter")]
    public class ThingParameterDAL
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [StringLength(500)]
        public string Value { get; set; }

        public DateTime Timestamp { get; set; }

        public int ThingId { get; set; }

        [ForeignKey("ThingId")]
        public ThingDAL Thing { get; set; }

    }
}
