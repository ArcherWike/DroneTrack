using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Data;
using DroneTrack.Source.Layouts;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;
using Microsoft.Web.WebView2.Core;


namespace DroneTrack.Source.ViewModels
{
    partial class ManagementViewModel : ModuleViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<FlightLog> _allFlights = new();

        public Action<string>? ExecuteJavaScript { get; set; }

        public ManagementViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadFlightsData();
        }

        public void LoadFlightsData()
        {
            using (var db = new DroneDatabaseContext())
            {
                var databaseDrones = db.FlightLogs.ToList();

                _allFlights.Clear();
                foreach (var d in databaseDrones)
                {
                    _allFlights.Add(d);
                }
            }
        }


        //Details panel visibility
        [ObservableProperty]
        private bool _isDetailVisible = false;

        [ObservableProperty]
        public FlightLog _selectedFlight;

        partial void OnSelectedFlightChanged(FlightLog value)
        {
            IsDetailVisible = value != null;
        }

        //Filter
        [ObservableProperty]
        private DateTime? _selectedDate = DateTime.Today;

        partial void OnSelectedDateChanged(DateTime? value)
        {
            ApplyFilter();
        }

        [ObservableProperty]
        private TimeSpan _selectedStart = TimeSpan.Zero;

        partial void OnSelectedStartChanged(TimeSpan value)
        {
            ApplyFilter();
        }

        [ObservableProperty]
        private TimeSpan _selectedEnd = TimeSpan.FromHours(24);

        partial void OnSelectedEndChanged(TimeSpan value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (!SelectedDate.HasValue) return;

            DateTime startFull = SelectedDate.Value.Add(SelectedStart);
            DateTime endFull = SelectedDate.Value.Add(SelectedEnd);

            var results = _databaseService.GetFlightsByRange(startFull, endFull);

            // UI Update
            AllFlights.Clear();
            foreach (var flight in results)
            {
                AllFlights.Add(flight);
            }
        }

        private void ApplySpatialFilter(double centerLat, double centerLng, double radiusInMeters)
        {
            var results = _databaseService.GetFlightsBySpatialFilter(centerLat, centerLng, radiusInMeters);
        }


        [RelayCommand]
        private void ClearFilters()
        {
            SelectedStart = TimeSpan.Zero;
            SelectedEnd = TimeSpan.FromHours(24);
            //SelectedDate = DateTime.Today;
        }

        protected override void RegisterForMessages()
        {
            base.RegisterForMessages();

            WeakReferenceMessenger.Default.Register<MapSpatialFilterMessage>(this, (r, m) =>
            {
                ApplySpatialFilter(m.CenterLat, m.CenterLng, m.RadiusInMeters);
            });

            WeakReferenceMessenger.Default.Register<MapReadyMessage>(this, (r, m) =>
            {

            });
        }
    }
}
