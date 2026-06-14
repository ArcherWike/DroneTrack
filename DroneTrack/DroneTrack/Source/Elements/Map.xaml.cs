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

                if (message?.type == "MAP_READY")
                {
                    WeakReferenceMessenger.Default.Send(new MapReadyMessage());

                    string script = $"clearAllMarkers();";
                    MapView.CoreWebView2.ExecuteScriptAsync(script);
                }

                if (message?.type == "NEW_MARKER")
                {
                    AddNewMarker(message);
                }

                if (message?.type == "SPATIAL_FILTER")
                {
                    SendSpatialFilterMessage(message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd Mapy: {ex.Message}");
            }
        }

        private void AddNewMarker(MapMessage message)
        {
            double lat = message.data.lat;
            double lng = message.data.lng;

            WeakReferenceMessenger.Default.Send(new MapClickedMessage(lat, lng));

            System.Diagnostics.Debug.WriteLine($"Mapa wysłała wiadomość: {lat}, {lng}");
        }

        private void SendSpatialFilterMessage(MapMessage message)
        {
            double centerLat = message.data.lat;
            double centerLng = message.data.lng;
            double radius = message.data.radius;

            WeakReferenceMessenger.Default.Send(new MapSpatialFilterMessage(centerLat, centerLng, radius));
        }
    }
}
