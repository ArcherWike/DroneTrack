using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using DroneTrack.Source.Models;
using Microsoft.Data.Sqlite;

namespace DroneTrack.Source.Data
{
    public class DatabaseService
    {
        DroneDatabaseContext database = new DroneDatabaseContext();

        public DatabaseService()
        {
            database.Database.EnsureCreated();
        }

        public List<FlightLog> GetFlightsByRange(DateTime start, DateTime end)
        {
            using (var db = new DroneDatabaseContext())
            {
                var allFlights = db.FlightLogs.ToList();
                return allFlights.Where(f => f.FlightDateStart >= start && f.FlightDateEnd <= end).ToList();
            }                
        }
    }
}
