using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;

using DroneTrack.Source.Data;
using DroneTrack.Source.Layouts;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;


namespace DroneTrack.Source.ViewModels
{
    partial class ManagementViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private bool _isDetailVisible = false;

        [ObservableProperty]
        private ObservableCollection<FlightLog> _allFlights = new();

        public Action<string>? ExecuteJavaScript { get; set; }

        public ManagementViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            LoadFlights();
        }

        public void LoadFlights()
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
    }   
}
