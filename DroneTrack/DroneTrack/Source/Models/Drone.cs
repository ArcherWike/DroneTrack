using System.ComponentModel.DataAnnotations;

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
