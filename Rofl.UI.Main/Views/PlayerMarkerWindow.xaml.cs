using Rofl.Settings.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for PlayerMarkerWindow.xaml
    /// </summary>
    public partial class PlayerMarkerWindow : Window
    {

        private bool _isEditMode;
        private PlayerMarker _marker;
        private string _oldName;

        public PlayerMarkerWindow()
        {
            _isEditMode = false;
            InitializeComponent();

            this.Title = TryFindResource("AddButtonText") as String + " " + this.Title;

        }

        public PlayerMarkerWindow(PlayerMarker marker)
        {
            InitializeComponent();

            if (marker != null)
            {
                _isEditMode = true;
                _marker = marker;
                _oldName = _marker.Name;

                this.Title = TryFindResource("EditButtonText") as String + " " + this.Title;
                NameTextBox.Text = _marker.Name;
                MarkerColorPicker.SelectedColor = ColorConverter.ConvertFromString(_marker.Color) as Color?;
            }
            else
            {
                this.Title = TryFindResource("AddButtonText") as String + " " + this.Title;
                _isEditMode = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ObservableCollection<PlayerMarker> context)) { return; }

            string inputName = NameTextBox.Text;
            string colorText = MarkerColorPicker.SelectedColorText;

            // Validate if input information is OK
            if (String.IsNullOrWhiteSpace(inputName))
            {
                MessageBox.Show
                (
                    TryFindResource("PlayerMarkerNameIsEmptyErrorText") as String,
                    TryFindResource("ErrorTitle") as String,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            if (String.IsNullOrWhiteSpace(colorText))
            {
                MessageBox.Show
                (
                    TryFindResource("PlayerMarkerColorIsEmptyErrorText") as String,
                    TryFindResource("ErrorTitle") as String,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            // Check if name already exists
            if (!_isEditMode)   // If we are creating a new item, no need to check old name collision
            {
                var existingItem = context.Where
                    (
                        x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase)
                    ).FirstOrDefault();

                // Name does not exist
                if (existingItem != null)
                {
                    MessageBox.Show
                    (
                        (TryFindResource("PlayerMarkerAlreadyExistsErrorText") as String).Replace("$", inputName),
                        TryFindResource("PlayerMarkerAlreadyExistsErrorTitle") as String,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                // New one, add it!!!!!
                var newMarker = new PlayerMarker
                {
                    Name = inputName,
                    Color = colorText
                };
                context.Add(newMarker);
                this.Close();
            }
            else
            {
                // Make sure you aren't changing the name to another marker
                var existingItem = context.Where
                    (
                        x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase) &&
                             !x.Name.Equals(_oldName, StringComparison.OrdinalIgnoreCase)
                    ).FirstOrDefault();

                // Name does not exist
                if (existingItem != null)
                {
                    MessageBox.Show
                    (
                        (TryFindResource("PlayerMarkerAlreadyExistsErrorText") as String).Replace("$", inputName),
                        TryFindResource("PlayerMarkerAlreadyExistsErrorTitle") as String,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                // New one, add it!!!!!
                _marker.Name = inputName;
                _marker.Color = colorText;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
