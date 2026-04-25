using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DroneTrack.Source.Data;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace DroneTrack.Source.ViewModels
{
    public partial class AddFlightViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Drone> _availableDrones;

        [ObservableProperty]
        private Drone _selectedDrone;

        // Dane przekazane z mapy
        [ObservableProperty] private double _lat;
        [ObservableProperty] private double _lng;
        [ObservableProperty] private string _operatorId;
        [ObservableProperty] private string _currentTime = DateTime.Now.ToString("HH:mm:ss");
        [ObservableProperty] private int _delayMinutes = 0;
        [ObservableProperty] private int _durationMinutes = 30;
        [ObservableProperty] private int _maxAltitude = 120;
        [ObservableProperty] private string _description = "";



        public AddFlightViewModel(DatabaseService dbService, double lat, double lng)
        {
            _databaseService = dbService;
            _lat = lat;
            _lng = lng;

            LoadDrones();
        }

        void LoadDrones()
        {
            using var db = new DroneDatabaseContext();
            AvailableDrones = new ObservableCollection<Drone>(db.Drones.ToList());
        }

        [RelayCommand]
        private void SaveFlight(Window window)
        {
            if (_selectedDrone == null)
            {
                MessageBox.Show("Statek powietrzny nie został wybrany!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(_operatorId))
            {
                MessageBox.Show("Błędny identyfikator operatora!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_durationMinutes <= 0)
            {
                MessageBox.Show("Czas trwania lotu musi być większy niż 0 minut.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_delayMinutes < 0)
            {
                MessageBox.Show("Minimalne opóźnienie startu to 0 minut.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_maxAltitude <= 0 || _maxAltitude > 500)
            {
                MessageBox.Show("Maksymalna wysokość musi być większa niż 0 metrów i mniejsza niż 500 metrów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var db = new DroneDatabaseContext();

            var newFlight = new FlightLog
            {
                DroneId = _selectedDrone.Id,
                Latitude = _lat,
                Longitude = _lng,
                OperatorIdentity = int.Parse(_operatorId),
                FlightDateStart = DateTime.Now.AddMinutes(_delayMinutes),
                FlightDateEnd = DateTime.Now.AddMinutes(_delayMinutes + _durationMinutes),
                MaxAltitude = _maxAltitude,
                Description = _description
            };

            db.FlightLogs.Add(newFlight);
            db.SaveChanges();

            WeakReferenceMessenger.Default.Send(new AddMarkerMessage(SelectedDrone.Id, _lat, _lng, _durationMinutes));

            window.DialogResult = true;
            window.Close();
        }
    }
}
