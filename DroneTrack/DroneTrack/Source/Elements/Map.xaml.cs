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
using DroneTrack.Source.ViewModels;
using Microsoft.Web.WebView2.Core;


namespace DroneTrack.Source.Elements
{
    /// <summary>
    /// klasa hostująca komponent WebView2, zarządza dwukierunkową komunikacją między logiką C# a mapą JavaScript
    /// </summary>
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

            WeakReferenceMessenger.Default.Register<AddMarkerMessage>(this, (r, m) =>
            {
                Dispatcher.BeginInvoke(new Action (async () =>
                {
                    if (!this.IsVisible || MapView == null || MapView.CoreWebView2 == null)
                        return;

                    try
                    {
                        if (MapView?.CoreWebView2 != null)
                        {
                            var culture = System.Globalization.CultureInfo.InvariantCulture;
                            string script = $"addMarker({m.MissionId}, {m.Lat.ToString(culture)}, {m.Lng.ToString(culture)}, {m.DurationSeconds.ToString(culture)}, {m.DelaySeconds.ToString(culture)});";
                            await MapView.CoreWebView2.ExecuteScriptAsync(script);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wysyłania wiadomości do WebView2: {ex.Message}");
                        return;
                    }
                }));
            });

            WeakReferenceMessenger.Default.Register<AddFilteredMarkerMessage>(this, (r, m) =>
            {
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!this.IsVisible || MapView == null || MapView.CoreWebView2 == null)
                        return;

                    try
                    {
                        if (MapView?.CoreWebView2 != null)
                        {
                            var culture = System.Globalization.CultureInfo.InvariantCulture;
                            string script = $"addFilteredMarker({m.MissionId}, {m.Lat.ToString(culture)}, {m.Lng.ToString(culture)});";
                            await MapView.CoreWebView2.ExecuteScriptAsync(script);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wysyłania wiadomości do WebView2: {ex.Message}");
                        return;
                    }
                }));
            });

            WeakReferenceMessenger.Default.Register<UpdateFilteredMarkersOnMapMessage>(this, (r, m) =>
            {
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!this.IsVisible || MapView == null || MapView.CoreWebView2 == null)
                        return;

                    try
                    {
                        if (MapView?.CoreWebView2 != null)
                        {
                            var culture = System.Globalization.CultureInfo.InvariantCulture;
                            string json = JsonSerializer.Serialize(m.MarkersId);
                            string script = $"filterDrones({json});";
                            await MapView.CoreWebView2.ExecuteScriptAsync(script);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wysyłania wiadomości do WebView2: {ex.Message}");
                        return;
                    }
                }));
            });

            WeakReferenceMessenger.Default.Register<UIDroneSelectedMessage>(this, (r, m) =>
            {
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!this.IsVisible || MapView == null || MapView.CoreWebView2 == null)
                        return;

                    try
                    {
                        if (MapView?.CoreWebView2 != null)
                        {
                            var culture = System.Globalization.CultureInfo.InvariantCulture;
                            string json = JsonSerializer.Serialize(m.DroneId);
                            string script = $"selectDrone({json});";
                            await MapView.CoreWebView2.ExecuteScriptAsync(script);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wysyłania wiadomości do WebView2: {ex.Message}");
                        return;
                    }
                }));
            });

            WeakReferenceMessenger.Default.Register<ClearMapMarkersMessage>(this, (r, m) =>
            {
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!this.IsVisible || MapView == null || MapView.CoreWebView2 == null)
                        return;

                    try
                    {
                        if (MapView?.CoreWebView2 != null)
                        {
                            ClearMap();
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wysyłania wiadomości do WebView2: {ex.Message}");
                        return;
                    }
                }));
            });


            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = System.IO.Path.Combine(baseDirectory, "Source", "map.html");

            MapView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                var message = JsonSerializer.Deserialize<MapMessage>(json);

                if (message is null)
                {
                    System.Diagnostics.Debug.WriteLine("Otrzymano nieprawidłową wiadomość z mapy.");
                    return;
                }

                if (message?.type == "MAP_READY")
                {
                    ClearMap();
                    WeakReferenceMessenger.Default.Send(new MapReadyMessage());
                }

                if (message.data.ValueKind == JsonValueKind.Undefined || message.data.ValueKind == JsonValueKind.Null)
                {
                    System.Diagnostics.Debug.WriteLine("Otrzymano nieprawidłową wiadomość z mapy.");
                    return;
                }

                if (message?.type == "NEW_MARKER")
                {
                    AddNewMarker(message.data);
                }

                if (message?.type == "SPATIAL_FILTER")
                {
                    SendSpatialFilterMessage(message.data);
                }

                if (message?.type == "DRONE_CLICKED")
                {
                    OnDroneClicked(message.data);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd Mapy: {ex.Message}");
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
            MapSpatialFilterMessage spatialFilterMessage = data.Deserialize<MapSpatialFilterMessage>();
            if (spatialFilterMessage is not null)
            {
                WeakReferenceMessenger.Default.Send(new MapSpatialFilterMessage(spatialFilterMessage.CenterLat, spatialFilterMessage.CenterLng, spatialFilterMessage.RadiusInMeters));
            }
        }
    }
}
