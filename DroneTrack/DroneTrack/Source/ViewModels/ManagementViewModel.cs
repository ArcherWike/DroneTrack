using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Data;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;


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

        private void AddFlightsToMap()
        {
            using (var db = new DroneDatabaseContext())
            {
                var databaseDrones = db.FlightLogs.ToList();

                foreach (var d in databaseDrones)
                {
                    WeakReferenceMessenger.Default.Send(new AddFilteredMarkerMessage(d.Id, d.Latitude, d.Longitude));
                }
            }
        }

        private void OnFlightsDataChanged()
        {
            var databaseDrones = AllFlights.ToList();

            List<int> markerIds = new List<int>();
            foreach (var d in databaseDrones)
            {
                markerIds.Add(d.Id);
                WeakReferenceMessenger.Default.Send(new AddFilteredMarkerMessage(d.Id, d.Latitude, d.Longitude));
            }

            WeakReferenceMessenger.Default.Send(new UpdateFilteredMarkersOnMapMessage(markerIds));
        }

        private CancellationTokenSource? _filterDebounceToken;

        private void DebounceFilters()
        {
            _filterDebounceToken?.Cancel();
            _filterDebounceToken?.Dispose();

            _filterDebounceToken = new CancellationTokenSource();
            var token = _filterDebounceToken.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(500, token);

                    if (!token.IsCancellationRequested)
                    {
                        OnFlightsDataChanged();
                    }
                }
                catch (TaskCanceledException)
                {
                }
            });
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

            DebounceFilters();
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
                AddFlightsToMap();
            });
        }
    }
}
