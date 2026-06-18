using System.Text.Json;

namespace DroneTrack.Source.Messages
{
    //---------- Map Data Messages ----------
    public class MapMessage
    {
        public string type { get; set; }
        public JsonElement data { get; set; }
    }
    
    //---------- Communication Messages ----------

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
        public int MissionId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public int DurationSeconds { get; set; }
        public int DelaySeconds { get; set; }
        public AddMarkerMessage(int missionId, double lat, double lng, int durationSeconds, int delaySeconds)
        {
            MissionId = missionId;
            Lat = lat;
            Lng = lng;
            DurationSeconds = durationSeconds;
            DelaySeconds = delaySeconds;
        }
    }

    public class AddFilteredMarkerMessage
    {
        public int MissionId { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public AddFilteredMarkerMessage(int missionId, double lat, double lng)
        {
            MissionId = missionId;
            Lat = lat;
            Lng = lng;
        }
    }

    public class MapAddSpatialFilterMessage
    {
        public double CenterLat { get; set; }
        public double CenterLng { get; set; }
        public double RadiusInMeters { get; set; }

        public MapAddSpatialFilterMessage(double centerLat, double centerLng, double radiusInMeters)
        {
            CenterLat = centerLat;
            CenterLng = centerLng;
            RadiusInMeters = radiusInMeters;
        }
    }

    public class MapRemoveSpatialFilterMessage {}


    public class MapReadyMessage { }

    public class ClearMapMarkersMessage { }

    public class ClearMapSpatialFilterMessage{
        
    }

    public class UpdateFilteredMarkersOnMapMessage
    {
        public List<int> MarkersId;

        public UpdateFilteredMarkersOnMapMessage(List<int> markersId)
        {
            MarkersId = markersId;
        }
    }

    public class DroneClickedMessage
    {
        public int DroneId { get; set; }
        public DroneClickedMessage(int droneId)
        {
            DroneId = droneId;
        }
    }

    public class UIDroneSelectedMessage
    {
        public int DroneId { get; set; }
        public UIDroneSelectedMessage(int droneId)
        {
            DroneId = droneId;
        }
    }

    public class ManagementModeChangedMessage
    {
        public bool IsEnabled { get; set; }

        public ManagementModeChangedMessage(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}
