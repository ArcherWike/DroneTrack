using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Data;
using DroneTrack.Source.Layouts;
using DroneTrack.Source.Messages;

namespace DroneTrack.Source.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        private AddFlightWindow? flightWindow;
        public UserViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            RegisterForMessages();
        }

        void RegisterForMessages()
        {
            WeakReferenceMessenger.Default.Register<MapClickedMessage>(this, (r, m) =>
            {
                OnMapClicked(m.Lat, m.Lng);
            });

            WeakReferenceMessenger.Default.Register<MapReadyMessage>(this, (r, m) =>
            {
                LoadActiveFlights();
            });
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
            using (var db = new DroneDatabaseContext())
            {
                var currentTime = DateTime.Now;

                var activeFlights = db.FlightLogs
                    .Where(f => f.FlightDateEnd > currentTime)
                    .ToList();

                foreach (var lot in activeFlights)
                {
                    var delay = (int)(lot.FlightDateStart - currentTime).TotalSeconds;
                    var totalTime = (int)(lot.FlightDateEnd - currentTime).TotalSeconds;

                    if (delay < 0) delay = 0;

                    if (totalTime > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Lot dodany, pozostalo ", totalTime);

                        WeakReferenceMessenger.Default.Send(new AddMarkerMessage(
                            lot.Id,
                            lot.Latitude,
                            lot.Longitude,
                            totalTime,
                            delay));
                    }
                }
            }
        }
    }
}