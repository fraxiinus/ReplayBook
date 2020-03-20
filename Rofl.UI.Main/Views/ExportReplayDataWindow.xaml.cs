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

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ReplayExportDataWindow.xaml
    /// </summary>
    public partial class ExportReplayDataWindow : Window
    {
        private readonly CollectionViewSource _levelThreeView;

        private readonly ObservableCollection<ExportSelectItemModel> _levelOneItems;
        private readonly ObservableCollection<ExportSelectItemModel> _levelTwoItems;
        private readonly ObservableCollection<ExportSelectItemModel> _levelThreeItems;
        private bool _csvMode = false;

        public ExportReplayDataWindow()
        {
            InitializeComponent();

            _levelOneItems = new ObservableCollection<ExportSelectItemModel>();
            this.LevelOneSelectBox.ItemsSource = _levelOneItems;

            _levelTwoItems = new ObservableCollection<ExportSelectItemModel>();
            this.LevelTwoSelectBox.ItemsSource = _levelTwoItems;

            _levelThreeItems = new ObservableCollection<ExportSelectItemModel>();
            _levelThreeView = new CollectionViewSource { Source = _levelThreeItems };
            this.LevelThreeSelectBox.ItemsSource = _levelThreeView.View;
        }

        private void ExportReplayDataWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ReplayFile replay))
            {
                return;
            }

            foreach (var property in replay.GetType().GetProperties())
            {
                if (property.Name.Equals("BluePlayers", StringComparison.InvariantCulture))
                {
                    continue;
                }

                if (property.Name.Equals("RedPlayers", StringComparison.InvariantCulture))
                {
                    continue;
                }

                _levelOneItems.Add(new ExportSelectItemModel()
                {
                    Name = property.Name,
                    Checked = false
                });
            }

            foreach (var player in replay.Players)
            {
                _levelTwoItems.Add(new ExportSelectItemModel()
                {
                    Name = $"{player.SKIN} - {player.NAME}",
                    InternalString = player.NAME,
                    Checked = false
                });
            }

            foreach (var property in typeof(Player).GetProperties())
            {
                if (property.Name.Equals("Id", StringComparison.InvariantCulture))
                {
                    continue;
                }

                if (property.Name.Equals("PlayerID", StringComparison.InvariantCulture))
                {
                    continue;
                }

                _levelThreeItems.Add(new ExportSelectItemModel()
                {
                    Name = property.Name,
                    Checked = false
                });
            }
        }

        private void LevelOne_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }

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

            var test = checkBox.DataContext as ExportSelectItemModel;

            this.LevelThreeSelectBox.IsEnabled = _levelTwoItems.Any(x => x.Checked);
        }

        private void LevelThree_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) { return; }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportButton_OnClick(object sender, RoutedEventArgs e)
        {
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

            var results = _csvMode ? ConstructCsvResults() : JsonConvert.SerializeObject(ConstructJsonResults());

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

                saveDialog.Filters.Add(_csvMode
                    ? new CommonFileDialogFilter("CSV Files", "*.csv")
                    : new CommonFileDialogFilter("JSON Files", "*.json"));

                if (saveDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

                try
                {
                    var targetFile = saveDialog.FileName;
                    File.WriteAllText(targetFile, results);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(TryFindResource("ErdExportNullTest") as string,
                        TryFindResource("ErdFailedToSave") as string + "\n" + ex.ToString(),
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
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

        private void SelectAll(ObservableCollection<ExportSelectItemModel> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (var item in list)
            {
                item.Checked = true;
            }
        }

        private void DeselectAll(ObservableCollection<ExportSelectItemModel> list)
        {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            foreach (var item in list)
            {
                item.Checked = false;
            }
        }

        private Dictionary<string, object> ConstructJsonResults()
        {
            if (!(this.DataContext is ReplayFile replay)) { return null; }

            // Include level one items
            var kvpResults = new Dictionary<string, object>();
            foreach (var rootItem in _levelOneItems)
            {
                if (!rootItem.Checked) continue;
                if (rootItem.Name.Equals("Players", StringComparison.InvariantCulture))
                {
                    kvpResults.Add(rootItem.Name, "Yes");
                    continue;
                }

                var value = replay.GetType().GetProperty(rootItem.Name)?.GetValue(replay);
                kvpResults.Add(rootItem.Name, value);
            }

            // Populate level two items
            if (kvpResults.ContainsKey("Players") && LevelTwoSelectBox.IsEnabled)
            {
                var playerDict = new Dictionary<string, object>();
                kvpResults["Players"] = playerDict;

                foreach (var player in _levelTwoItems)
                {
                    if (player.Checked)
                    {
                        playerDict.Add
                            (
                                player.InternalString, "Yes"
                            );
                    }
                }

                // Populate level three items
                if (playerDict.Count > 0 && LevelThreeSelectBox.IsEnabled)
                {
                    foreach (var playerName in _levelTwoItems.Where(x => x.Checked)
                        .Select(x => x.InternalString))
                    {
                        playerDict[playerName] = GetPlayerProperties(replay.Players
                            .FirstOrDefault(x => x.NAME.Equals(playerName, StringComparison.InvariantCulture)));
                    }
                }
            }

            return kvpResults;
        }

        private string ConstructCsvResults()
        {
            if (!(this.DataContext is ReplayFile replay)) { return null; }
            var lines = new List<List<string>>();

            // Add selected properties as first line
            var selectedProps = _levelThreeItems.Where(x => x.Checked).Select(x => x.Name).ToList();
            lines.Add(selectedProps);

            foreach (var player in _levelTwoItems)
            {
                if (!player.Checked) continue;

                var playerData = replay.Players.FirstOrDefault(x =>
                    x.NAME.Equals(player.InternalString, StringComparison.InvariantCulture));

                var values = selectedProps.Select(prop =>
                    playerData?.GetType().GetProperty(prop)?.GetValue(playerData) as string).ToList();

                lines.Add(values);
            }

            string result = string.Empty;
            foreach (var line in lines)
            {
                result += string.Join(",", line) + "\n";
            }

            return result;
        }

        public IEnumerable<dynamic> GetPlayerProperties(Player player)
        {
            var selectedProperties = _levelThreeItems.Where(x => x.Checked).Select(x => x.Name);

            var values = selectedProperties.Select(prop => new
            {
                Name = prop,
                Value = player.GetType().GetProperty(prop)?.GetValue(player) as string
            });

            return values;
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

            if (!(e.Item is ExportSelectItemModel src))
                e.Accepted = false;
            else if (src.Name != null && !src.Name.Contains(filterText.ToUpper(CultureInfo.InvariantCulture)))// here is FirstName a Property in my YourCollectionItem
                e.Accepted = false;
        }
    }
}
