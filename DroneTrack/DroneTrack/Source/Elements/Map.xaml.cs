using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;
using DroneTrack.Source.ViewModels;
using Microsoft.Web.WebView2.Core;


namespace DroneTrack.Source.Elements
{
    /// klasa hostująca komponent WebView2, 
    /// zarządza dwukierunkową komunikacją między logiką C# a mapą JavaScript

    public partial class Map : UserControl
    {
        public Map()
        {
            InitializeComponent();
            InitializeWebView();

            this.Unloaded += (s, e) =>
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);
            };
        }

        private async void InitializeWebView()
        {
            await MapView.EnsureCoreWebView2Async(null);

            MapView.CoreWebView2.WebMessageReceived += WebMessageReceived;

            WeakReferenceMessenger.Default.Register<AddMarkerMessage>(
                this, AddMarkerToMap);

            WeakReferenceMessenger.Default.Register<UpdateFilteredMarkersOnMapMessage>(
                this, UpdateFilteredMarkersOnMap);

            WeakReferenceMessenger.Default.Register<AddFilteredMarkerMessage>(
                this, AddFilteredMarkerToMap);

            WeakReferenceMessenger.Default.Register<UIDroneSelectedMessage>(
                this, DroneSelectedOnMap);

            WeakReferenceMessenger.Default.Register<ManagementModeChangedMessage>(
                this, ChangeViewModeOnMap);

            WeakReferenceMessenger.Default.Register<ClearMapSpatialFilterMessage>(
                this, ClearSpatialFilterOnMap);

            WeakReferenceMessenger.Default.Register<ClearMapMarkersMessage>(
                this, ClearMarkersOnMap);


            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = System.IO.Path.Combine(baseDirectory, "Source", "map.html");

            MapView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private void ClearMarkersOnMap(object recipient, ClearMapMarkersMessage message)
        {
            string script = $"clearAllMarkers();";
            RunScript(script);
        }

        private void AddFilteredMarkerToMap(object recipient, AddFilteredMarkerMessage message)
        {
            string json = JsonSerializer.Serialize(message.list);
            string escaped = json.Replace("\\", "\\\\").Replace("\"", "\\\"");
            string script = $"addFilteredMarkers({json});";
            RunScript(script);
        }

        private void UpdateFilteredMarkersOnMap(object recipient, UpdateFilteredMarkersOnMapMessage message)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string json = JsonSerializer.Serialize(message.MarkersId);
            string script = $"filterDrones({json});";
            RunScript(script);
        }

        private void ChangeViewModeOnMap(object recipient, ManagementModeChangedMessage message)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string script = $"setSpatialFilterMode({message.IsEnabled.ToString().ToLower()});";
            RunScript(script);
        }

        private void ClearSpatialFilterOnMap(object recipient, ClearMapSpatialFilterMessage message)
        {
            string script = $"clearSpatialFilter();";
            RunScript(script);
        }

        private void DroneSelectedOnMap(object recipient, UIDroneSelectedMessage message)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string json = JsonSerializer.Serialize(message.DroneId);
            string script = $"selectDrone({json});";
            RunScript(script);
        }

        private void AddMarkerToMap(object recipient, AddMarkerMessage message)
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string script = $"addMarker({message.MissionId}, {message.Lat.ToString(culture)}, {message.Lng.ToString(culture)}, {message.DurationSeconds.ToString(culture)}, {message.DelaySeconds.ToString(culture)});";

            RunScript(script);
        }

        private async void RunScript(string script)
        {
            try
            {
                await MapView.Dispatcher.InvokeAsync(async () =>
                {
                    await MapView.CoreWebView2.ExecuteScriptAsync(script);
                });
            }
            catch (Exception exception)
            {
                SendMessageError(exception);
            }
        } 

        private void SendMessageError(Exception exception)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Błąd podczas wysyłania wiadomości do WebView2: {exception.Message}");
#endif
        }

        private void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                var message = JsonSerializer.Deserialize<MapMessage>(json);

                if (message is null)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Otrzymano nieprawidłową wiadomość z mapy.");
#endif
                    return;
                }

                if (message?.type == "MAP_READY")
                {
                    ClearMap();
                    WeakReferenceMessenger.Default.Send(new MapReadyMessage());
                    return;
                }

                if (message?.type == "REMOVE_SPATIAL_FILTER")
                {
                    WeakReferenceMessenger.Default.Send(new MapRemoveSpatialFilterMessage());
                    return;
                }
                if (message.data.ValueKind == JsonValueKind.Undefined || message.data.ValueKind == JsonValueKind.Null)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Otrzymano nieprawidłową wiadomość z mapy.");
#endif
                    return;
                }

                if (message?.type == "NEW_MARKER")
                {
                    AddNewMarker(message.data);
                }
                else if (message?.type == "ADD_SPATIAL_FILTER")
                {
                    SendSpatialFilterMessage(message.data);
                }
                else if (message?.type == "DRONE_CLICKED")
                {
                    OnDroneClicked(message.data);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Błąd Mapy: {ex.Message}");
#endif
            }
        }

        private void ClearMap()
        {
            string script = $"clearAllMarkers();";
            MapView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void OnDroneClicked(JsonElement data)
        {
            var droneClickedMessage = data.Deserialize<DroneClickedMessage>();
            if (droneClickedMessage is not null)
            {
                WeakReferenceMessenger.Default.Send(new DroneClickedMessage(droneClickedMessage.DroneId));
            }
        }

        private void AddNewMarker(JsonElement data)
        {
            MapClickedMessage mapData = data.Deserialize<MapClickedMessage>();
            if (mapData is not null)
            {
                WeakReferenceMessenger.Default.Send(new MapClickedMessage(mapData.Lat, mapData.Lng));
            }

            // System.Diagnostics.Debug.WriteLine($"Mapa wysłała wiadomość: {mapData.Lat}, {mapData.Lng}");
        }

        private void SendSpatialFilterMessage(JsonElement data)
        {
            MapAddSpatialFilterMessage spatialFilterMessage = data.Deserialize<MapAddSpatialFilterMessage>();
            if (spatialFilterMessage is not null)
            {
                WeakReferenceMessenger.Default.Send(new MapAddSpatialFilterMessage(spatialFilterMessage.CenterLat, spatialFilterMessage.CenterLng, spatialFilterMessage.RadiusInMeters));
            }
        }
    }
}
