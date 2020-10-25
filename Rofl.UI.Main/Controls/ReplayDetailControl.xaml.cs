using Rofl.UI.Main.Models;
using Rofl.UI.Main.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rofl.UI.Main.Controls
{
    /// <summary>
    /// Interaction logic for ReplayDetailView.xaml
    /// </summary>
    public partial class ReplayDetailControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReplayDetailControl()
        {
            InitializeComponent();
        }

        private Thickness _statsPlayerIconMargin;
        public Thickness StatsPlayerIconMargin
        {
            get => _statsPlayerIconMargin;
            set
            {
                _statsPlayerIconMargin = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(StatsPlayerIconMargin)));
            }
        }

        private void ReplayDetailControlElement_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            PlayerIconsGrid.Children.Clear();
            PlayerIconsGrid.ColumnDefinitions.Clear();

            if (DetailTabControl.SelectedIndex == 1)
            {
                int counter = 0;
                foreach (var player in replay.AllPlayers)
                {
                    // Add a column
                    var newColumn = new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    };
                    PlayerIconsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var newImage = new PlayerIconControl
                    {
                        DataContext = player.PreviewModel,
                        Width = 50,
                        Height = 50
                    };
                    Grid.SetRow(newImage, 0);
                    Grid.SetColumn(newImage, counter);
                    PlayerIconsGrid.Children.Add(newImage);

                    counter++;
                }
            }
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            await context.PlayReplay(replay.PreviewModel).ConfigureAwait(true);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is TabControl context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            StatsScrollViewer.ScrollToVerticalOffset(0);

            if (context.SelectedIndex == 1 && PlayerIconsGrid.ColumnDefinitions.Count < 1)
            {
                int counter = 0;
                foreach (var player in replay.AllPlayers)
                {
                    // Add a column
                    var newColumn = new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    };
                    PlayerIconsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var newImage = new PlayerIconControl
                    {
                        DataContext = player.PreviewModel,
                        Width = 50,
                        Height = 50
                    };
                    Grid.SetRow(newImage, 0);
                    Grid.SetColumn(newImage, counter);
                    PlayerIconsGrid.Children.Add(newImage);

                    counter++;
                }
            }
        }

        private void StatsScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            StatsScrollViewer.ScrollToVerticalOffset(0);
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button moreButton)) { return; }

            // Get the button and menu
            ContextMenu contextMenu = moreButton.ContextMenu;
            // Set placement and open
            contextMenu.PlacementTarget = moreButton;
            contextMenu.Placement = PlacementMode.Left;
            contextMenu.IsOpen = true;
        }

        private void OpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            context.OpenReplayContainingFolder(replay.PreviewModel.Location);
        }

        private void ViewOnlineMatchHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!(Window.GetWindow(this)?.DataContext is MainWindowViewModel context)) { return; }
            if (!(this.DataContext is ReplayDetail replay)) { return; }

            context.ViewOnlineMatchHistory(replay.PreviewModel.MatchId);
        }
    }
}
