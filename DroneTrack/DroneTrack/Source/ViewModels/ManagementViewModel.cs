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
        private readonly DatabaseService databaseService;

        [ObservableProperty]
        private ObservableCollection<FlightLog> allFlights = new();

        public Action<string>? ExecuteJavaScript { get; set; }

        public ManagementViewModel(DatabaseService _databaseService)
        {
            databaseService = _databaseService;
        }

        public void LoadFlightsData()
        {
            List<FlightLog> databaseDrones = GetDronesByTimeFilter();
            List<MarkerData> markerDatas = new List<MarkerData>();
            AllFlights.Clear();
            foreach (var d in databaseDrones)
            {
                AllFlights.Add(d);
                markerDatas.Add(new MarkerData(d.Id, d.Latitude, d.Longitude));
            }
            WeakReferenceMessenger.Default.Send(new AddFilteredMarkerMessage(markerDatas));
        }

        private void OnFlightsDataChanged()
        {
            var databaseDrones = AllFlights.ToList();

            List<MarkerData> markerDatas = new List<MarkerData>();
            List<int> markerIds = new List<int>();
            foreach (var d in databaseDrones)
            {
                markerIds.Add(d.Id);
                markerDatas.Add(new MarkerData(d.Id, d.Latitude, d.Longitude));

            }
            WeakReferenceMessenger.Default.Send(new AddFilteredMarkerMessage(markerDatas));
            WeakReferenceMessenger.Default.Send(new UpdateFilteredMarkersOnMapMessage(markerIds));
        }

        private CancellationTokenSource? filterDebounceToken;

        private void DebounceFilters()
        {
            filterDebounceToken?.Cancel();
            filterDebounceToken?.Dispose();

            filterDebounceToken = new CancellationTokenSource();
            var token = filterDebounceToken.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100, token);

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
        private bool isDetailVisible = false;

        [ObservableProperty]
        private FlightLog selectedFlight;
        [ObservableProperty]
        private Drone selectedFlightDrone;

        partial void OnSelectedFlightChanged(FlightLog value)
        {
            SelectedFlightDrone = databaseService.GetDroneById(value.DroneId);
            if (SelectedFlight != null)
            {
                WeakReferenceMessenger.Default.Send(new UIDroneSelectedMessage(SelectedFlight.Id));
            }
            
            IsDetailVisible = value != null;
        }

        //Filter
        [ObservableProperty]
        private DateTime? _selectedDate = DateTime.Today;

        partial void OnSelectedDateChanged(DateTime? value)
        {
            ApplyDateFilter();
        }

        [ObservableProperty]
        private TimeSpan _selectedStart = TimeSpan.Zero;

        partial void OnSelectedStartChanged(TimeSpan value)
        {
            ApplyDateFilter();
        }

        [ObservableProperty]
        private TimeSpan _selectedEnd = TimeSpan.FromHours(24);

        partial void OnSelectedEndChanged(TimeSpan value)
        {
            ApplyDateFilter();
        }

        List<FlightLog> GetDronesByTimeFilter()
        {
            if (!SelectedDate.HasValue) return new List<FlightLog>();

            DateTime startFull = SelectedDate.Value.Add(SelectedStart);
            DateTime endFull = SelectedDate.Value.Add(SelectedEnd);

            return databaseService.GetFlightsByRange(startFull, endFull);
        }

        private void ApplyDateFilter()
        {
            if (!SelectedDate.HasValue) return;

            var results = GetDronesByTimeFilter();
            WeakReferenceMessenger.Default.Send(new ClearMapSpatialFilterMessage());
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
            if (!SelectedDate.HasValue) return;

            DateTime startFull = SelectedDate.Value.Add(SelectedStart);
            DateTime endFull = SelectedDate.Value.Add(SelectedEnd);

            List<FlightLog> results = databaseService.GetFlightsByFilters(startFull, endFull, centerLat, centerLng, radiusInMeters);

            AllFlights.Clear();
            foreach (var flight in results)
            {
                AllFlights.Add(flight);
            }
            OnFlightsDataChanged();
        }

        private void SelectClickedDrone(int id)
        {
            if (AllFlights == null) return;
            if (AllFlights.Count == 0) return; 

            FlightLog selected = AllFlights.FirstOrDefault(drone => drone.Id == id);
            if (selected != null)
            {
                SelectedFlight = selected;
            }
        }


        [RelayCommand]
        private void ClearFilters()
        {
            SelectedStart = TimeSpan.Zero;
            SelectedEnd = TimeSpan.FromHours(24);

            ApplyDateFilter();
        }

        protected override void RegisterForMessages()
        {
            base.RegisterForMessages();

            WeakReferenceMessenger.Default.Register<MapAddSpatialFilterMessage>(this, (r, m) =>
            {
                ApplySpatialFilter(m.CenterLat, m.CenterLng, m.RadiusInMeters);
            });

            WeakReferenceMessenger.Default.Register<MapRemoveSpatialFilterMessage>(this, (r, m) =>
            {
                ApplyDateFilter();
            });

            WeakReferenceMessenger.Default.Register<MapReadyMessage>(this, (r, m) =>
            {
                LoadFlightsData();
                WeakReferenceMessenger.Default.Send(new ManagementModeChangedMessage(true));
            });

            WeakReferenceMessenger.Default.Register<DroneClickedMessage>(this, (r, m) =>
            {
                SelectClickedDrone(m.DroneId);
            });
        }
        virtual public void CleanUp()
        {
            base.CleanUp();

            WeakReferenceMessenger.Default.Send(new ManagementModeChangedMessage(false));
        }
    }
}
