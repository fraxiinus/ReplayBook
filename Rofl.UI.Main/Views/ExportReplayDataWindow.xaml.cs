using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ReplayExportDataWindow.xaml
    /// </summary>
    public partial class ExportReplayDataWindow : Window
    {
        private readonly ObservableCollection<ExportSelectItemModel> _levelOneItems;
        private readonly ObservableCollection<ExportSelectItemModel> _levelTwoItems;
        private readonly ObservableCollection<ExportSelectItemModel> _levelThreeItems;

        public ExportReplayDataWindow()
        {
            InitializeComponent();

            _levelOneItems = new ObservableCollection<ExportSelectItemModel>();
            this.LevelOneSelectBox.ItemsSource = _levelOneItems;

            _levelTwoItems = new ObservableCollection<ExportSelectItemModel>();
            this.LevelTwoSelectBox.ItemsSource = _levelTwoItems;

            _levelThreeItems = new ObservableCollection<ExportSelectItemModel>();
            this.LevelThreeSelectBox.ItemsSource = _levelThreeItems;
        }

        private void ExportReplayDataWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ReplayFile replay)) { return; }

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
                    Checked = false
                });
            }

            foreach (var property in typeof(Player).GetProperties())
            {
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
            throw new NotImplementedException();
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
    }
}
