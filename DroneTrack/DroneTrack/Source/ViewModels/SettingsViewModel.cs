using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DroneTrack.Source.Models;
using DroneTrack.Source.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DroneTrack.Source.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        public SettingsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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

            _allDrones.Add(newDrone);
            _selectedDrone = newDrone;
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
                db.Drones.Remove(_selectedDrone);
                db.SaveChanges();
            }

            _allDrones.Remove(_selectedDrone);
            _selectedDrone = null;
        }
    }
}
