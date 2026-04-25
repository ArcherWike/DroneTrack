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

namespace DroneTrack.Source.Layouts
{
    /// <summary>
    /// Klasa reprezentująca widok ustawień aplikacji (SettingsView).
    /// </summary>
    /// Główne funkcjonalności:
    /// - Wyświetlanie i modyfikacja ustawień aplikacji, takich jak konfiguracja bazy danych, preferencje użytkownika itp.
    /// - Zarządzenie flotą za pomocą interaktywnego formularza
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }
    }
}
