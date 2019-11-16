using Rofl.Settings;
using Rofl.Settings.Models;
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
using System.Windows.Shapes;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // This window should open as a dialog, so set owner
            this.Owner = App.Current.MainWindow;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            context.SaveConfigFile();
            this.DialogResult = true;
        }

        private void SettingsMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((sender as ListBox).SelectedItem as ListBoxItem);

            switch (selectedItem.Name)
            {
                case "GeneralSettingsListItem":
                    SettingsTabControl.SelectedIndex = 0;
                    break;
                case "ReplaySettingsListItem":
                    SettingsTabControl.SelectedIndex = 1;
                    break;
                case "RequestSettingsListItem":
                    SettingsTabControl.SelectedIndex = 2;
                    break;
                default:
                    break;
            }
        }

        private void AddKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }

            var addDialog = new PlayerMarkerWindow
            {
                Top = this.Top + 50,
                Left = this.Left + 50,
                DataContext = context.Settings.KnownPlayers
            };

            addDialog.Show();

        }

        private void EditKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            var editDialog = new PlayerMarkerWindow(selectedPlayer)
            {
                Top = this.Top + 50,
                Left = this.Left + 50,
                DataContext = context.Settings.KnownPlayers
            };
            editDialog.Show();

        }

        private void RemoveKnownPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SettingsManager context)) { return; }
            if (!(KnownPlayersListBox.SelectedItem is PlayerMarker selectedPlayer)) { return; }

            context.Settings.KnownPlayers.Remove(selectedPlayer);

            EditKnownPlayerButton.IsEnabled = false;
            RemoveKnownPlayerButton.IsEnabled = false;
            //context.Settings.KnownPlayers.Remove("Etirps");
        }

        private void KnownPlayersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem as PlayerMarker == null) { return; };

            EditKnownPlayerButton.IsEnabled = true;
            RemoveKnownPlayerButton.IsEnabled = true;
        }
    }
}
