using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneTrack.Source.Models
{
    public class Drone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ModelType { get; set; }
    }
}
