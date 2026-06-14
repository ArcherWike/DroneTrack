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

        private readonly ManagementViewModel _managementVM;
        private readonly SettingsViewModel _settingsVM;
        private readonly UserViewModel _userVM;

        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            _managementVM = new ManagementViewModel(_databaseService);
            _settingsVM = new SettingsViewModel(_databaseService);
            _userVM = new UserViewModel(_databaseService);
            _currentView = _managementVM;
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            CurrentView = destination switch
            {
                "Management" => (object)_managementVM,
                "User" => _userVM,
                "Settings" => _settingsVM,
                _ => _managementVM
            };
        }
    }
}
