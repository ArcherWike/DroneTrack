using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace DroneTrack.Source.Elements
{
    /// <summary>
    /// Interaktywny suwak czasu umożliwiający filtrowanie danych 
    /// w zadanym przedziale godzinowym
    /// </summary>
    /// 
    /// Funkcjonalność obejmuje:
    /// - Dwupunktowy wybór przedziału czasowego.
    /// - Powiadamianie o zmianach zakresu (INotifyPropertyChanged).
    /// - Synchronizację z bazą danych poprzez ViewModel w celu filtrowania logów lotów.

    public partial class TimeRangeSlider : UserControl
    {
        // Dependency Property for time binding
        public static readonly DependencyProperty TimeFromProperty =
            DependencyProperty.Register("TimeFrom",
                typeof(TimeSpan), typeof(TimeRangeSlider),
                new FrameworkPropertyMetadata(
                    TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TimeSpan TimeFrom
        {
            get => (TimeSpan)GetValue(TimeFromProperty);
            set => SetValue(TimeFromProperty, value);
        }

        // Property TimeTo
        public static readonly DependencyProperty TimeToProperty =
            DependencyProperty.Register("TimeTo", typeof(TimeSpan), typeof(TimeRangeSlider),
                new FrameworkPropertyMetadata(TimeSpan.FromHours(1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public TimeSpan TimeTo
        {
            get => (TimeSpan)GetValue(TimeToProperty);
            set => SetValue(TimeToProperty, value);
        }

        public DateTime? GetSelectedDate()
        {
            //can be null if user didn't select any date
            return MissionDatePicker.SelectedDate;
        }

        public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(TimeRangeSlider),
        new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        private void OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDate = MissionDatePicker.SelectedDate;
            var newDate = SelectedDate;
        }

        public (TimeSpan Start, TimeSpan End) GetTimeRange()
        {
            if (EndSlider.Value >= 1440)//24*minutes per hour
            {
                return (TimeSpan.FromMinutes(StartSlider.Value), TimeSpan.FromMinutes(1439));
            }

            return (
                TimeSpan.FromMinutes(StartSlider.Value),
                TimeSpan.FromMinutes(EndSlider.Value)
            );
        }

        public string GetFormattedData()
        {
            var date = GetSelectedDate()?.ToShortDateString() ?? "No Date";
            var range = GetTimeRange();
            return $"{date} between {range.Start:hh\\:mm} and {range.End:hh\\:mm}";
        }

        private const double MinGap = 15;
        public TimeRangeSlider()
        {
            InitializeComponent();

            StartSlider.ValueChanged += OnStartSliderValueChanged;
            EndSlider.ValueChanged += OnEndSliderValueChanged;
            MissionDatePicker.SelectedDateChanged += OnSelectedDateChanged;

            MissionDatePicker.SelectedDate = DateTime.Today;
        }

        private void OnStartSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Blocking lower slider
            if (StartSlider.Value > EndSlider.Value - MinGap)
            {
                StartSlider.Value = EndSlider.Value - MinGap;
            }
            UpdateTimeLabels();
        }

        private void OnEndSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Blocking upper slider
            if (EndSlider.Value < StartSlider.Value + MinGap)
            {
                EndSlider.Value = StartSlider.Value + MinGap;
            }
            UpdateTimeLabels();
        }

        private void UpdateTimeLabels()
        {
            var range = GetTimeRange();

            string startTimeString = range.Start.ToString(@"hh\:mm");
            string endTimeString = range.End.ToString(@"hh\:mm");

            // Update UI TextBlock
            TimeLabel.Text = $"{startTimeString} - {endTimeString}";

            TimeFrom = range.Start;
            TimeTo = range.End;
           
            //System.Diagnostics.Debug.WriteLine(GetFormattedData());
        }
    }
}
