using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DroneTrack.Source.ViewModels;
using DroneTrack.Source.Layouts;

namespace DroneTrack.Source.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Obiekty widoków są tworzone raz i trzymane w pamięci
        private readonly ManagementViewModel _managementVM = new ManagementViewModel();
        private readonly UserViewModel _userVM = new UserViewModel();
        private readonly SettingsViewModel _settingsVM = new SettingsViewModel();

        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {
            _currentView = _managementVM; // Widok startowy
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            if(destination == "Management") CurrentView = new ManagementViewModel();
            else if (destination == "User") CurrentView = new UserViewModel();
            //CurrentView = destination switch
            //{
            //    "Management" => _managementVM,
            //    "User" => _userVM,
            //    "Settings" => _settingsVM,
            //    _ => _managementVM
            //};
        }
    }
}
