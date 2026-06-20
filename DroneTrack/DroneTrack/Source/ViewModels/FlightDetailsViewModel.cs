using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneTrack.Source.Data;
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

        public void SetDroneById(int droneId)
        {
            FlightDrone = AvailableDrones?.FirstOrDefault(d => d.Id == droneId);
        }

        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }
    }
}
