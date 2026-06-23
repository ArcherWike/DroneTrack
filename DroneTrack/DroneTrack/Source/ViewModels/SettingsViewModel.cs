using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DroneTrack.Source.Data;
using DroneTrack.Source.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.IO;

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
            var newDrone = new Drone { ModelType = "New Drone (Edit name)" };

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
            "Are you sure you want to delete ALL data? This action cannot be undone!",
            "Critical Warning",
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

                    MessageBox.Show("All data has been successfully deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void UpdateDatabase()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Select JSON file with records"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string jsonString = File.ReadAllText(openFileDialog.FileName);

                    List<FlightLog> record = JsonSerializer.Deserialize<List<FlightLog>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (record == null || record.Count == 0)
                    {
                        MessageBox.Show("The JSON file is empty or has an invalid format.", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    using (var db = new DroneDatabaseContext())
                    {
                        db.FlightLogs.AddRange(record);
                        db.SaveChanges();
                    }

                    MessageBox.Show($"Success! Successfully added {record.Count} records to the database.", "Import Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (JsonException)
                {
                    MessageBox.Show("Error: The selected file has an invalid JSON structure. Make sure it matches the database model.", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred during import: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
