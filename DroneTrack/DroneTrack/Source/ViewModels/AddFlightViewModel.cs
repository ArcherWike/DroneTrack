using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneTrack.Source.Data;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace DroneTrack.Source.ViewModels
{
    //Form for flight details
    //After successful verification, the data is added to the database

    public partial class AddFlightViewModel : ObservableObject
    {
        private readonly DatabaseService databaseService;

        [ObservableProperty]
        private ObservableCollection<Drone> _availableDrones;
        [ObservableProperty]
        private Drone _selectedDrone;

        // Map data
        [ObservableProperty] private double _lat;
        [ObservableProperty] private double _lng;
        // Form data
        [ObservableProperty] private string _operatorId;
        [ObservableProperty] private string _currentTime = DateTime.Now.ToString("HH:mm:ss");
        [ObservableProperty] private int _delayMinutes = 0;
        [ObservableProperty] private int _durationMinutes = 30;
        [ObservableProperty] private int _maxAltitude = 120;
        [ObservableProperty] private string _description = "";

        public AddFlightViewModel(DatabaseService dbService, double lat, double lng)
        {
            databaseService = dbService;
            Lat = lat;
            Lng = lng;

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
            if (SelectedDrone == null)
            {
                MessageBox.Show("Statek powietrzny nie został wybrany!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(OperatorId))
            {
                MessageBox.Show("Błędny identyfikator operatora!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (DurationMinutes <= 0)
            {
                MessageBox.Show("Czas trwania lotu musi być większy niż 0 minut.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (DelayMinutes < 0)
            {
                MessageBox.Show("Minimalne opóźnienie startu to 0 minut.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MaxAltitude <= 0 || MaxAltitude > 500)
            {
                MessageBox.Show("Maksymalna wysokość musi być większa niż 0 metrów i mniejsza niż 500 metrów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var db = new DroneDatabaseContext();

            var newFlight = new FlightLog
            {
                DroneId = SelectedDrone.Id,
                Latitude = Lat,
                Longitude = Lng,
                OperatorIdentity = OperatorId,
                FlightDateStart = DateTime.Now.AddMinutes(DelayMinutes),
                FlightDateEnd = DateTime.Now.AddMinutes(DelayMinutes + DurationMinutes),
                MaxAltitude = MaxAltitude,
                Description = Description
            };

            databaseService.AddNewFlight(newFlight);

            WeakReferenceMessenger.Default.Send(new AddMarkerMessage(newFlight.Id, Lat, Lng, DurationMinutes * 60, DelayMinutes * 60));
            window.Close();
        }
    }
}
