using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneTrack.Source.Messages
{
    //---------- Map Data Messages ----------
    public class MapMessage
    {
        public string type { get; set; }
        public MapData data { get; set; }
    }

    public class MapData
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public double radius { get; set; }
        public string action { get; set; }
    }
    //---------- User Interaction Messages ----------

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

    public class AddMarkerMessage
    {
        public int DroneId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public int DurationMinutes { get; set; }

        public AddMarkerMessage(int droneId, double lat, double lng, int durationMinutes)
        {
            DroneId = droneId;
            Lat = lat;
            Lng = lng;
            DurationMinutes = durationMinutes;
        }
    }

    public class MapReadyMessage { }
}
