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

        public List<FlightLog> GetFlightsBySpatialFilter(double centerLat, double centerLng, double radiusInMeters)
        {
            using (var db = new DroneDatabaseContext())
            {
                var allFlights = db.FlightLogs.ToList();
                return allFlights.Where(f => IsWithinRadius(f.Latitude, f.Longitude, centerLat, centerLng, radiusInMeters)).ToList();
            }
        }
        private bool IsWithinRadius(double lat1, double lng1, double lat2, double lng2, double radiusInMeters)
        {
            double R = 6371000; // Radius of the Earth in meters
            double dLat = (lat2 - lat1) * (Math.PI / 180);
            double dLng = (lng2 - lng1) * (Math.PI / 180);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return distance <= radiusInMeters;
        }
    }
}