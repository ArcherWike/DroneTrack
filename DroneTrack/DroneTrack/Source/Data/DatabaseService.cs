using DroneTrack.Source.Models;

namespace DroneTrack.Source.Data
{
    public class DatabaseService
    {
        DroneDatabaseContext database = new DroneDatabaseContext();

        public DatabaseService()
        {
            database.Database.EnsureCreated();
        }

        public Drone GetDroneById(int id)
        {
            using (var db = new DroneDatabaseContext())
            {
                return db.Drones.FirstOrDefault(d => d.Id == id);
            }
        }

        public List<Drone> GetAllDrones()
        {
            using (var db = new DroneDatabaseContext())
            {
                return db.Drones.ToList();
            }
        }

        public void UpdateFlightLog(FlightLog flight)
        {
            using (var db = new DroneDatabaseContext())
            {
                var existingFlight = db.FlightLogs.FirstOrDefault(f => f.Id == flight.Id);
                if (existingFlight != null)
                {
                    existingFlight.OperatorIdentity = flight.OperatorIdentity;
                    existingFlight.DroneId = flight.DroneId;
                    existingFlight.MaxAltitude = flight.MaxAltitude;
                    existingFlight.Description = flight.Description;
                    db.SaveChanges();
                }
            }
        }

        public void AddNewFlight(FlightLog flight)
        {
            using (var db = new DroneDatabaseContext())
            {
                db.FlightLogs.Add(flight);
                db.SaveChanges();
            }
        }

        public List<FlightLog> GetActiveFlights()
        {
            using (var db = new DroneDatabaseContext())
            {
                var currentTime = DateTime.Now;

                return db.FlightLogs
                    .Where(flight => flight.FlightDateEnd > currentTime)
                    .ToList();
            }
        }

        public List<FlightLog> GetFlightsByFilters(DateTime start, DateTime end, double centerLat, double centerLng, double radiusInMeters)
        {
            var filteredByTime = GetFlightsByRange(start, end);

            return filteredByTime.Where(f => IsWithinRadius(f.Latitude, f.Longitude, centerLat, centerLng, radiusInMeters)).ToList();
        }

        public List<FlightLog> GetFlightsByRange(DateTime start, DateTime end)
        {
            using (var db = new DroneDatabaseContext())
            {
                return db.FlightLogs.Where(f => f.FlightDateStart >= start && f.FlightDateEnd <= end).ToList();
            }
        }

        public List<FlightLog> GetFlightsBySpatialFilter(double centerLat, double centerLng, double radiusInMeters)
        {
            using (var db = new DroneDatabaseContext())
            {
                return db.FlightLogs.Where(f => IsWithinRadius(f.Latitude, f.Longitude, centerLat, centerLng, radiusInMeters)).ToList();
            }
        }
        private bool IsWithinRadius(double lat1, double lng1, double lat2, double lng2, double radiusInMeters)
        {
            double R = 6371000; // Radius of the Earth in meters

            //Haversine formula to find distance between two points on a sphere
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