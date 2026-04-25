using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneTrack.Source.Messages
{
    public class MapClickedMessage
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public MapClickedMessage(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }

    // 2. Wiadomość: "Dodaj nowy znacznik na mapie" (Z ViewModeli -> do Mapy)
    public class AddMarkerMessage
    {
        public int DroneId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public AddMarkerMessage(int droneId, double lat, double lng)
        {
            DroneId = droneId;
            Lat = lat;
            Lng = lng;
        }
    }
}
