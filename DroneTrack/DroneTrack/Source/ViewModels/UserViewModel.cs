using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Data;
using DroneTrack.Source.Layouts;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;

namespace DroneTrack.Source.ViewModels
{
    public partial class UserViewModel : ModuleViewModel
    {
        private readonly DatabaseService _databaseService;

        private AddFlightWindow? flightWindow;
        public UserViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

        }
        private void OnMapClicked(double lat, double lng)
        {
            if (flightWindow != null) return;

            var addFlightVm = new AddFlightViewModel(_databaseService, lat, lng);

            flightWindow = new AddFlightWindow();
            flightWindow.Closed += (s, e) =>
            {
                flightWindow = null;
            };
            flightWindow.DataContext = addFlightVm;

            flightWindow.Show();
        }

        private void LoadActiveFlights()
        {
            DateTime currentTime = DateTime.Now;

            List<FlightLog> activeFlights = _databaseService.GetActiveFlights();

            foreach (var lot in activeFlights)
            {
                var delay = (int)(lot.FlightDateStart - currentTime).TotalSeconds;
                var totalTime = (int)(lot.FlightDateEnd - currentTime).TotalSeconds;

                if (delay < 0) delay = 0;

                if (totalTime > 0)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Lot dodany, pozostalo ", totalTime);
#endif
                    WeakReferenceMessenger.Default.Send(new AddMarkerMessage(
                        lot.Id,
                        lot.Latitude,
                        lot.Longitude,
                        totalTime,
                        delay));
                }
            }
        }
        protected override void RegisterForMessages()
        {
            base.RegisterForMessages();

            WeakReferenceMessenger.Default.Register<MapClickedMessage>(this, (r, m) =>
            {
                OnMapClicked(m.Lat, m.Lng);
            });

            WeakReferenceMessenger.Default.Register<MapReadyMessage>(this, (r, m) =>
            {
                LoadActiveFlights();
            });
        }
    }
}