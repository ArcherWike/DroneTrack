using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DroneTrack.Source.ViewModels;
using DroneTrack.Source.Layouts;
using DroneTrack.Source.Data;

namespace DroneTrack.Source.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        // Obiekty widoków są tworzone raz i trzymane w pamięci
        private readonly ManagementViewModel _managementVM;
        private readonly SettingsViewModel _settingsVM;

        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            _managementVM = new ManagementViewModel(_databaseService);
            _settingsVM = new SettingsViewModel(_databaseService);
            _currentView = _managementVM;
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            CurrentView = destination switch
            {
                "Management" => (object)_managementVM,
                "User" => new UserViewModel(_databaseService),
                "Settings" => _settingsVM,
                _ => _managementVM
            };
        }
    }
}
