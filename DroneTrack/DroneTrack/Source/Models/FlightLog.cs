using System.ComponentModel.DataAnnotations;


namespace DroneTrack.Source.Models
{
    public class FlightLog
    {
        public string FullDisplayName => $"[{Id}] {FlightDateStart}";

        [Key]
        public int Id { get; set; }
        [Required]
        public required string OperatorIdentity { get; set; }
        [Required]
        public int DroneId { get; set; }
        [Required]
        public DateTime FlightDateStart { get; set; } = DateTime.Now;
        [Required]
        public DateTime FlightDateEnd { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double MaxAltitude { get; set; }
        public string Description { get; set; }
    }
}
