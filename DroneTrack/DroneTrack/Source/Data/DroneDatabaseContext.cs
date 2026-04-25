using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

using DroneTrack.Source.Models;

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
