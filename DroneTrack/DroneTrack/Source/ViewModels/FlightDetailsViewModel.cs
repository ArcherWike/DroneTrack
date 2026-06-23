using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Data;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;

namespace DroneTrack.Source.ViewModels
{
    public partial class FlightDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService databaseService;
        public FlightDetailsViewModel(DatabaseService dbService)
        {
            databaseService = dbService;
            AvailableDrones = new ObservableCollection<Drone>(dbService.GetAllDrones());
            FlightDrone = AvailableDrones?.FirstOrDefault();
        }

        [ObservableProperty]
        private ObservableCollection<Drone> availableDrones;

        [ObservableProperty]
        private Drone flightDrone;

        [ObservableProperty]
        private bool isEditMode = false;

        //Details panel visibility
        [ObservableProperty]
        private bool isDetailVisible = false;

        [ObservableProperty]
        private FlightLog selectedFlight;

        partial void OnSelectedFlightChanged(FlightLog value)
        {
            if (SelectedFlight != null)
            {
                FlightDateStartText = SelectedFlight.FlightDateStart.ToString("dd.MM.yyyy HH:mm");
                FlightDateEndText = SelectedFlight.FlightDateEnd.ToString("dd.MM.yyyy HH:mm");
                LatitudeText = SelectedFlight.Latitude.ToString(CultureInfo.InvariantCulture);
                LongitudeText = SelectedFlight.Longitude.ToString(CultureInfo.InvariantCulture);
                MaxAltitudeText = SelectedFlight.MaxAltitude.ToString(CultureInfo.InvariantCulture);
                SetDroneById(value.DroneId);
                WeakReferenceMessenger.Default.Send(new UIDroneSelectedMessage(SelectedFlight.Id));
            }

            IsDetailVisible = value != null;
        }

        partial void OnFlightDroneChanged(Drone value)
        {
            if (SelectedFlight != null && FlightDrone != null)
            {
                SelectedFlight.DroneId = FlightDrone.Id;
            }
        }

        public void SetDroneById(int droneId)
        {
            FlightDrone = AvailableDrones?.FirstOrDefault(d => d.Id == droneId);
        }

        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }


        [RelayCommand]
        private void Save()
        {
            if (SelectedFlight == null) return;

            if (string.IsNullOrEmpty(SelectedFlight.OperatorIdentity))
            {
                MessageBox.Show("Invalid operator ID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedFlight.FlightDateStart > SelectedFlight.FlightDateEnd)
            {
                MessageBox.Show("Invalid flight start date!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (FlightDrone != null && !AvailableDrones.Contains(FlightDrone))
            {
                MessageBox.Show("Invalid operator ID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            databaseService.UpdateFlightLog(SelectedFlight);
            WeakReferenceMessenger.Default.Send(new OnRecordUpdatedMessage());
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedFlight == null) return;
            var result = MessageBox.Show("Are you sure you want to delete this flight?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                databaseService.DeleteFlightLog(SelectedFlight.Id);
                IsDetailVisible = false;
                IsEditMode = false;
            }
        }

        [ObservableProperty]
        private bool isStartDateValid = true;

        [ObservableProperty]
        private bool isEndDateValid = true;

        [ObservableProperty]
        private string flightDateStartText;

        [ObservableProperty]
        private string flightDateEndText;

        [ObservableProperty]
        private string latitudeText;

        [ObservableProperty]
        private bool isLatitudeValid = true;

        [ObservableProperty]
        private string longitudeText;

        [ObservableProperty]
        private bool isLongitudeValid = true;

        [ObservableProperty]
        private string maxAltitudeText;

        [ObservableProperty]
        private bool isMaxAltitudeValid = true;

        private bool ValidateAndParseDate(string value)
        {
            if (DateTime.TryParseExact(
            value,
            "dd.MM.yyyy HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var dt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        partial void OnFlightDateStartTextChanged(string value)
        {
            if (ValidateAndParseDate(value))
            {
                SelectedFlight.FlightDateStart = DateTime.ParseExact(value, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                IsStartDateValid = true;
            }
            else
            {
                IsStartDateValid = false;
            }
        }

        
        partial void OnFlightDateEndTextChanged(string value)
        {
            if (ValidateAndParseDate(value))
            {
                SelectedFlight.FlightDateEnd = DateTime.ParseExact(value, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                IsEndDateValid = true;
            }
            else
            {
                IsEndDateValid = false;
            }
        }

        partial void OnLatitudeTextChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            {
                SelectedFlight.Latitude = d;
                IsLatitudeValid = true;
            }
            else
            {
                IsLatitudeValid = false;
            }
        }

        partial void OnLongitudeTextChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            {
                SelectedFlight.Longitude = d;
                IsLongitudeValid = true;
            }
            else
            {
                IsLongitudeValid = false;
            }
        }

        partial void OnMaxAltitudeTextChanged(string value)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                if (result < 0 || result > 500)
                {
                    IsMaxAltitudeValid = false;
                    return;
                }
                SelectedFlight.MaxAltitude = result;
                IsMaxAltitudeValid = true;
            }
            else
            {
                IsMaxAltitudeValid = false;
            }
        }
    }
}
