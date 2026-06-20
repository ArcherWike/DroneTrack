using System.IO;
using DroneTrack.Source.Models;
using Microsoft.EntityFrameworkCore;

namespace DroneTrack.Source.Data
{
    class DroneDatabaseContext : DbContext
    {
        public DbSet<Drone> Drones { get; set; }
        public DbSet<FlightLog> FlightLogs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "drone_data.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
