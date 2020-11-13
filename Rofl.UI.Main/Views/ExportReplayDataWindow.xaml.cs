using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Rofl.UI.Main.Utilities;
using System.Diagnostics;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ReplayExportDataWindow.xaml
    /// </summary>
    public partial class ExportReplayDataWindow : Window
    {
        private readonly CollectionViewSource _levelThreeView;

        private readonly ObservableCollection<ExportSelectItem> _levelOneItems;
        private readonly ObservableCollection<ExportSelectItem> _levelTwoItems;
        private readonly ObservableCollection<ExportSelectItem> _levelThreeItems;
        private bool _csvMode = false;
        private ReplayFile _replay;

        public ExportReplayDataWindow()
        {
            InitializeComponent();

            // Create the collections of select objects for each level
            // Assign select boxes item source
            _levelOneItems = new ObservableCollection<ExportSelectItem>();
            this.LevelOneSelectBox.ItemsSource = _levelOneItems;

            _levelTwoItems = new ObservableCollection<ExportSelectItem>();
            this.LevelTwoSelectBox.ItemsSource = _levelTwoItems;

            _levelThreeItems = new ObservableCollection<ExportSelectItem>();
            // Create a view to allow searching
            _levelThreeView = new CollectionViewSource { Source = _levelThreeItems };
            this.LevelThreeSelectBox.ItemsSource = _levelThreeView.View;
        }

        private void ExportReplayDataWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ExportContext context)) { return; }

            // We only support one replay right now
            if (!(context.Replays.FirstOrDefault() is ReplayFile replay)) { return; }
            _replay = replay;

            // Load first level properties
            foreach (var property in _replay.GetType().GetProperties())
            {

                // Ignore seperate team data
                if (property.Name.Equals("BluePlayers", StringComparison.InvariantCulture)) continue;
                if (property.Name.Equals("RedPlayers", StringComparison.InvariantCulture)) continue;

                // Load first level properties
                _levelOneItems.Add(new ExportSelectItem()
                {
                    Name = property.Name,
                    InternalString = property.Name,
                    Checked = false,
                    Value = property.GetValue(_replay).ToString()
                });
            }

            // Load second level properties, champions and player names
            foreach (var player in _replay.Players)
            {
                _levelTwoItems.Add(new ExportSelectItem()
                {
                    Name = player.NAME,
                    InternalString = player.NAME,
                    Checked = false,
                    Value = player.SKIN
                });
            }

            // Load initial third level properties, player properties
            // We need to assign the property values dynamically
            var playerProps = typeof(Player).GetProperties().OrderBy(x => x.Name);
            foreach (var property in playerProps)
            {
                if (property.Name.Equals("Id", StringComparison.InvariantCulture)) continue;
                if (property.Name.Equals("PlayerID", StringComparison.InvariantCulture)) continue;

                _levelThreeItems.Add(new ExportSelectItem()
                {
                    Name = property.Name,
                    InternalString = property.Name,
                    Checked = false,
                    Value = "N/A"
                });
            }
        }

        private void LevelOne_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

            // Update preview box
            Update_PreviewStringTextBox();

            if (!(checkBox.Content as String).Equals("Players", StringComparison.InvariantCulture)) return;

            // If we are disabling level 2, we have to disable level 3
            if (LevelTwoSelectBox.IsEnabled)
            {
                this.LevelThreeSelectBox.IsEnabled = false;
            }
            // If we are enabling level 2, we have to enable level 3 if there are items checked
            else if (_levelTwoItems.Any(x => x.Checked))
            {
                this.LevelThreeSelectBox.IsEnabled = true;
            }

            this.LevelTwoSelectBox.IsEnabled = !this.LevelTwoSelectBox.IsEnabled;
        }

        private void LevelTwo_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

            // Update preview box
            Update_PreviewStringTextBox();

            var selectedName = (checkBox.Content as TextBlock).Text;

            this.LevelTwoSelectBox.SelectedItem = _levelTwoItems.First(x => x.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase));

            this.LevelThreeSelectBox.IsEnabled = _levelTwoItems.Any(x => x.Checked);
        }

        // Change the preview values for the third column when the player selection changed
        private void LevelTwoSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(this.DataContext is ExportContext context)) { return; }

            // We only support one replay right now
            // if (!(context.Replays.FirstOrDefault() is ReplayFile replay)) { return; }


            var playerName = ((ExportSelectItem) this.LevelTwoSelectBox.SelectedItem).Name;
            var player = _replay.Players.First(x => x.NAME.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            foreach (var property in _levelThreeItems)
            {
                // Get value can be null
                property.Value = player.GetType().GetProperty(property.Name).GetValue(player)?.ToString() ?? "N/A";
            }
        }

        private void LevelThree_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

            // Update preview box
            Update_PreviewStringTextBox();
        }

        private string Update_PreviewStringTextBox()
        {
            string exportString;
            if (_csvMode)
            {
                exportString = ExportHelper.ConstructCsvString(_replay, _levelTwoItems.ToList(), _levelThreeItems.ToList());
            }
            else
            {
                exportString = ExportHelper.ConstructJsonString(_replay, _levelOneItems.ToList(), _levelTwoItems.ToList(), _levelThreeItems.ToList());
            }

            this.PreviewStringTextBox.Text = exportString;
            return exportString;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ExportContext context)) { return; }
            if (!(context.Replays.FirstOrDefault() is ReplayFile replay)) { return; }

            // none are checked, nothing to export
            if (_csvMode)
            {
                if (!_levelTwoItems.Any(x => x.Checked))
                {
                    MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                        TryFindResource("ErdExportNullTitle") as string,
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    return;
                }
            }
            else if (!_levelOneItems.Any(x => x.Checked))
            {
                MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                                TryFindResource("ErdExportNullTitle") as string,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }

            var results = Update_PreviewStringTextBox();

            using (var saveDialog = new CommonSaveFileDialog())
            {
                saveDialog.Title = TryFindResource("ErdExportDialogTitle") as string;
                saveDialog.AddToMostRecentlyUsedList = false;
                saveDialog.EnsureFileExists = true;
                saveDialog.EnsurePathExists = true;
                saveDialog.EnsureReadOnly = false;
                saveDialog.EnsureValidNames = true;
                saveDialog.ShowPlacesList = true;

                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.DefaultFileName = replay.MatchId + (_csvMode ? ".csv" : ".json");

                saveDialog.Filters.Add(_csvMode
                    ? new CommonFileDialogFilter("CSV Files", "*.csv")
                    : new CommonFileDialogFilter("JSON Files", "*.json"));

                if (saveDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                try
                {
                    var targetFile = saveDialog.FileName;
                    File.WriteAllText(targetFile, results);

                    // Open the folder and select the file that was made
                    Process.Start("explorer.exe", $"/select, \"{targetFile}\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                        TryFindResource("ErdFailedToSave") as string + "\n" + ex.ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }

                this.Close();
            }
        }

        private void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) { return; }
            if (!(menuItem.Parent is ContextMenu parentBox)) { return; }

            switch (parentBox.Name)
            {
                case "LevelOneContextMenu":
                {
                    SelectAll(_levelOneItems);
                    break;
                }
                case "LevelTwoContextMenu":
                {
                    SelectAll(_levelTwoItems);
                    break;
                }
                case "LevelThreeContextMenu":
                {
                    SelectAll(_levelThreeItems);
                    break;
                }
                default:
                {
                    return;
                }
            }
        }

        private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) { return; }
            if (!(menuItem.Parent is ContextMenu parentBox)) { return; }

            switch (parentBox.Name)
            {
                case "LevelOneContextMenu":
                {
                    DeselectAll(_levelOneItems);
                    break;
                }
                case "LevelTwoContextMenu":
                {
                    DeselectAll(_levelTwoItems);
                    break;
                }
                case "LevelThreeContextMenu":
                {
                    DeselectAll(_levelThreeItems);
                    break;
                }
                default:
                {
                    return;
                }
            }
        }

        private void SelectAll(ObservableCollection<ExportSelectItem> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (var item in list)
            {
                item.Checked = true;
            }
        }

        private void DeselectAll(ObservableCollection<ExportSelectItem> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (var item in list)
            {
                item.Checked = false;
            }
        }

        private void CsvModeCheckbox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

            if (checkBox.IsChecked != null) _csvMode = (bool) checkBox.IsChecked;

            if (_csvMode)
            {
                // Enable csv mode
                LevelOneSelectBox.IsEnabled = false;
                LevelTwoSelectBox.IsEnabled = true;

                if (_levelTwoItems.Any(x => x.Checked))
                {
                    LevelThreeSelectBox.IsEnabled = true;
                }
            }
            else
            {
                // Disable
                var levelOneSwitch = _levelOneItems.FirstOrDefault(x => x.Name.Equals("Players", StringComparison.InvariantCulture));
                if (levelOneSwitch != null && levelOneSwitch.Checked)
                {
                    LevelTwoSelectBox.IsEnabled = true;
                }
                else
                {
                    LevelTwoSelectBox.IsEnabled = false;
                    LevelThreeSelectBox.IsEnabled = false;
                }

                LevelOneSelectBox.IsEnabled = true;
            }

            // Update preview box
            Update_PreviewStringTextBox();
        }

        private void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox textbox)) { return; }

            if (string.IsNullOrEmpty(textbox.Text))
            {
                _levelThreeView.Filter -= new FilterEventHandler(LevelThreeFilter);
                return;
            }

            _levelThreeView.Filter -= new FilterEventHandler(LevelThreeFilter);
            _levelThreeView.Filter += new FilterEventHandler(LevelThreeFilter);

            _levelThreeView.View.Refresh();
        }

        private void LevelThreeFilter(object sender, FilterEventArgs e)
        {
            var filterText = this.FilterTextBox.Text;

            if (!(e.Item is ExportSelectItem src))
                e.Accepted = false;
            else if (src.Name != null && !src.Name.Contains(filterText.ToUpper(CultureInfo.InvariantCulture)))// here is FirstName a Property in my YourCollectionItem
                e.Accepted = false;
        }
    }
}
