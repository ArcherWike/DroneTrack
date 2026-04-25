using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var addFlightVm = new AddFlightViewModel(_databaseService, lat, lng);

            var win = new AddFlightWindow
            {
                DataContext = addFlightVm
            };

            win.ShowDialog();
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
                    var timeLeft = (int)(lot.FlightDateEnd - currentTime).TotalMinutes;

                    if (timeLeft > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Lot dodany, pozostalo ", timeLeft);

                        WeakReferenceMessenger.Default.Send(new AddMarkerMessage(
                            lot.DroneId,
                            lot.Latitude,
                            lot.Longitude,
                            timeLeft));
                    }
                }
            }
        }
    }
}