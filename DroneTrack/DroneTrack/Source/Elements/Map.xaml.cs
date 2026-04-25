using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        public Map()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await MapView.EnsureCoreWebView2Async(null);

            MapView.CoreWebView2.WebMessageReceived += OnMapClick;

            WeakReferenceMessenger.Default.Register<AddMarkerMessage>(this, (r, m) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var culture = System.Globalization.CultureInfo.InvariantCulture;
                    string script = $"addMarker({m.DroneId}, {m.Lat.ToString(culture)}, {m.Lng.ToString(culture)});";
                    MapView.CoreWebView2.ExecuteScriptAsync(script);
                });
            });


            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = System.IO.Path.Combine(baseDirectory, "Source", "map.html");

            MapView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private void OnMapClick(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.WebMessageAsJson;
                var message = JsonSerializer.Deserialize<MapMessage>(json);

                if (message?.type == "NEW_MARKER")
                {
                    double lat = message.data.lat;
                    double lng = message.data.lng;

                    WeakReferenceMessenger.Default.Send(new MapClickedMessage(lat, lng));

                    System.Diagnostics.Debug.WriteLine($"Mapa wysłała wiadomość: {lat}, {lng}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd Mapy: {ex.Message}");
            }
        }
    }
}
