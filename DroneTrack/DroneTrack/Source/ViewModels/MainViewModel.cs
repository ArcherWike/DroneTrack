using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            _managementVM.Activate();
            _currentView = _managementVM;
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            if (destination == "Management")
            {
                if (CurrentView == _managementVM) return;

                _userVM.CleanUp();
                CurrentView = _managementVM;
                _managementVM.Activate();
            }
            else if (destination == "User")
            {
                if (CurrentView == _userVM) return;

                _managementVM.CleanUp();
                CurrentView = _userVM;
                _userVM.Activate();
            }
            else if (destination == "Settings")
            {
                _managementVM.CleanUp();
                _userVM.CleanUp();
                if (CurrentView == _settingsVM) return;
                CurrentView = _settingsVM;
            }
            else
            {
                _userVM.CleanUp();
                destination = "Management";
                _managementVM.Activate();
            }
        }
    }
}
