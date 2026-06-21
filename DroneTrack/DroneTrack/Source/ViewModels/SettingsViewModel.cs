using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneTrack.Source.Data;
using DroneTrack.Source.Models;
using Microsoft.EntityFrameworkCore;

namespace DroneTrack.Source.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly DatabaseService databaseService;
        public SettingsViewModel(DatabaseService _databaseService)
        {
            databaseService = _databaseService;
            LoadDrones();
            _selectedDrone = null;
        }

        [ObservableProperty]
        private ObservableCollection<Drone> _allDrones = new();

        [ObservableProperty]
        private Drone? _selectedDrone;

        public void LoadDrones()
        {
            using (var db = new DroneDatabaseContext())
            {
                var list = db.Drones.ToList();
                _allDrones = new ObservableCollection<Drone>(list);
            }
        }

        [RelayCommand]
        private void AddDrone()
        {
            var newDrone = new Drone { ModelType = "Nowy Dron (Edytuj nazwę)" };

            using (var db = new DroneDatabaseContext())
            {
                db.Drones.Add(newDrone);
                db.SaveChanges();
            }

            _selectedDrone = newDrone;
            _allDrones.Add(newDrone);
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (_selectedDrone == null) return;

            using (var db = new DroneDatabaseContext())
            {
                db.Drones.Update(_selectedDrone);
                db.SaveChanges();
            }
            LoadDrones();
        }

        [RelayCommand]
        private void DeleteDrone()
        {
            if (_selectedDrone == null) return;

            using (var db = new DroneDatabaseContext())
            {
                var droneInDb = db.Drones.FirstOrDefault(f => f.Id == _selectedDrone.Id);
                if (droneInDb != null)
                {
                    db.Drones.Remove(droneInDb);
                    db.SaveChanges();

                    _allDrones.Remove(_selectedDrone);
                    _selectedDrone = null;
                }
            }
        }

        //----------------< Database >-------------

        [RelayCommand]
        private async Task ClearDatabaseAsync()
        {
            var result = MessageBox.Show(
            "Czy na pewno chcesz usunąć WSZYSTKIE dane? Tej operacji nie można cofnąć!",
            "Krytyczne ostrzeżenie",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                using (var db = new DroneDatabaseContext())
                {
                    await db.FlightLogs.ExecuteDeleteAsync();
                }

                    MessageBox.Show("Wszystkie dane zostały pomyślnie usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
