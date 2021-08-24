using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Rofl.UI.Main.Utilities;
using System.Diagnostics;
using System.Reflection;

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
        private bool _csvMode;
        private ReplayFile _replay;

        public ExportReplayDataWindow()
        {
            InitializeComponent();
            _csvMode = false;

            // Create the collections of select objects for each level
            // Assign select boxes item source
            _levelOneItems = new ObservableCollection<ExportSelectItem>();
            LevelOneSelectBox.ItemsSource = _levelOneItems;

            _levelTwoItems = new ObservableCollection<ExportSelectItem>();
            LevelTwoSelectBox.ItemsSource = _levelTwoItems;

            _levelThreeItems = new ObservableCollection<ExportSelectItem>();
            // Create a view to allow searching
            _levelThreeView = new CollectionViewSource { Source = _levelThreeItems };
            LevelThreeSelectBox.ItemsSource = _levelThreeView.View;
        }

        private void ExportReplayDataWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportContext context)) { return; }

            // We only support one replay right now
            if (!(context.Replays.FirstOrDefault() is ReplayFile replay)) { return; }
            _replay = replay;

            // Load first level properties
            foreach (PropertyInfo property in _replay.GetType().GetProperties())
            {

                // Ignore seperate team data
                if (property.Name.Equals("BluePlayers", StringComparison.InvariantCulture)) { continue; }

                if (property.Name.Equals("RedPlayers", StringComparison.InvariantCulture)) { continue; }

                // Load first level properties
                _levelOneItems.Add(new ExportSelectItem()
                {
                    Name = property.Name,
                    InternalString = property.Name,
                    Checked = false,
                    Value = property.GetValue(_replay)?.ToString()
                });
            }

            // Load second level properties, champions and player names
            foreach (Player player in _replay.Players)
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
            IOrderedEnumerable<PropertyInfo> playerProps = typeof(Player).GetProperties().OrderBy(x => x.Name);
            foreach (PropertyInfo property in playerProps)
            {
                if (property.Name.Equals("Id", StringComparison.InvariantCulture)) { continue; }
                if (property.Name.Equals("PlayerID", StringComparison.InvariantCulture)) { continue; }

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
            _ = Update_PreviewStringTextBox();

            if (!(checkBox.Content as string).Equals("Players", StringComparison.InvariantCulture)) { return; }

            // If we are disabling level 2, we have to disable level 3
            if (LevelTwoSelectBox.IsEnabled)
            {
                LevelThreeSelectBox.IsEnabled = false;
            }
            // If we are enabling level 2, we have to enable level 3 if there are items checked
            else if (_levelTwoItems.Any(x => x.Checked))
            {
                LevelThreeSelectBox.IsEnabled = true;
            }

            LevelTwoSelectBox.IsEnabled = !LevelTwoSelectBox.IsEnabled;
        }

        private void LevelTwo_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

            // Update preview box
            _ = Update_PreviewStringTextBox();

            string selectedName = (checkBox.Content as TextBlock).Text;

            LevelTwoSelectBox.SelectedItem = _levelTwoItems.First(x => x.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase));

            LevelThreeSelectBox.IsEnabled = _levelTwoItems.Any(x => x.Checked);
        }

        // Change the preview values for the third column when the player selection changed
        private void LevelTwoSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(DataContext is ExportContext context)) { return; }

            // We only support one replay right now
            // if (!(context.Replays.FirstOrDefault() is ReplayFile replay)) { return; }


            string playerName = ((ExportSelectItem)LevelTwoSelectBox.SelectedItem).Name;
            Player player = _replay.Players.First(x => x.NAME.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            foreach (ExportSelectItem property in _levelThreeItems)
            {
                // Get value can be null
                property.Value = player.GetType().GetProperty(property.Name).GetValue(player)?.ToString() ?? "N/A";
            }
        }

        private void LevelThree_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox)) { return; }

            // Update preview box
            _ = Update_PreviewStringTextBox();
        }

        private string Update_PreviewStringTextBox()
        {
            string exportString = _csvMode
                ? ExportHelper.ConstructCsvString(_replay, _levelTwoItems.ToList(), _levelThreeItems.ToList())
                : ExportHelper.ConstructJsonString(_replay, _levelOneItems.ToList(), _levelTwoItems.ToList(), _levelThreeItems.ToList());

            PreviewStringTextBox.Text = exportString;
            return exportString;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportContext context)) { return; }
            if (!(context.Replays.FirstOrDefault() is ReplayFile replay)) { return; }

            // none are checked, nothing to export
            if (_csvMode)
            {
                if (!_levelTwoItems.Any(x => x.Checked))
                {
                    _ = MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                        TryFindResource("ErdExportNullTitle") as string,
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    return;
                }
            }
            else if (!_levelOneItems.Any(x => x.Checked))
            {
                _ = MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                                TryFindResource("ErdExportNullTitle") as string,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                return;
            }

            string results = Update_PreviewStringTextBox();

            using (CommonSaveFileDialog saveDialog = new CommonSaveFileDialog())
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

                if (saveDialog.ShowDialog() != CommonFileDialogResult.Ok) { return; }

                try
                {
                    string targetFile = saveDialog.FileName;
                    File.WriteAllText(targetFile, results);

                    // Open the folder and select the file that was made
                    _ = Process.Start("explorer.exe", $"/select, \"{targetFile}\"");
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                        TryFindResource("ErdFailedToSave") as string + "\n" + ex.ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }

                Close();
            }
        }

        private void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) { return; }
            if (!(menuItem.Parent is ContextMenu parentBox)) { return; }

            switch (parentBox.Name)
            {
                case "LevelOneContextMenu":
                    SelectAll(_levelOneItems);
                    break;
                case "LevelTwoContextMenu":
                    SelectAll(_levelTwoItems);
                    break;
                case "LevelThreeContextMenu":
                    SelectAll(_levelThreeItems);
                    break;
                default:
                    return;
            }
        }

        private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) { return; }
            if (!(menuItem.Parent is ContextMenu parentBox)) { return; }

            switch (parentBox.Name)
            {
                case "LevelOneContextMenu":
                    DeselectAll(_levelOneItems);
                    break;
                case "LevelTwoContextMenu":
                    DeselectAll(_levelTwoItems);
                    break;
                case "LevelThreeContextMenu":
                    DeselectAll(_levelThreeItems);
                    break;
                default:
                    return;
            }
        }

        private static void SelectAll(ObservableCollection<ExportSelectItem> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (ExportSelectItem item in list)
            {
                item.Checked = true;
            }
        }

        private static void DeselectAll(ObservableCollection<ExportSelectItem> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (ExportSelectItem item in list)
            {
                item.Checked = false;
            }
        }

        private void CsvModeCheckbox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

            if (checkBox.IsChecked != null) { _csvMode = (bool)checkBox.IsChecked; }

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
                ExportSelectItem levelOneSwitch = _levelOneItems.FirstOrDefault(x => x.Name.Equals("Players", StringComparison.InvariantCulture));
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
            _ = Update_PreviewStringTextBox();
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
            string filterText = FilterTextBox.Text;

            if (!(e.Item is ExportSelectItem src))
            {
                e.Accepted = false;
            }
            else if (src.Name != null && !src.Name.Contains(filterText.ToUpper(CultureInfo.InvariantCulture)))// here is FirstName a Property in my YourCollectionItem
            {
                e.Accepted = false;
            }
        }

        private void PreviewBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GridLength currentHeight = MainGrid.RowDefinitions[2].Height;
            double minHeight = MainGrid.RowDefinitions[2].MinHeight;

            MainGrid.RowDefinitions[2].Height = currentHeight.Value == minHeight
                ? new GridLength(200)
                : new GridLength(minHeight);
        }
    }
}
