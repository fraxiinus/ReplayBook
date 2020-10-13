using Rofl.Settings.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Rofl.UI.Main.Extensions;
using ModernWpf.Controls;
using Rofl.UI.Main.Utilities;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for PlayerMarkerWindow.xaml
    /// </summary>
    public partial class PlayerMarkerDialog : ContentDialog
    {
        private readonly PlayerMarker _marker;
        private readonly string _oldName;
        private readonly bool _isEditMode;

        private bool _blockClose = false;

        public PlayerMarkerDialog()
        {
            _isEditMode = false;
            InitializeComponent();

            this.Title = TryFindResource("AddButtonText") as String + " " + this.Title;
            MarkerColorPicker.SelectedColor = Colors.White;
        }

        public PlayerMarkerDialog(PlayerMarker marker)
        {
            InitializeComponent();

            if (marker != null)
            {
                _isEditMode = true;
                _marker = marker;
                _oldName = _marker.Name;

                this.Title = TryFindResource("EditButtonText") as String + " " + this.Title;
                NameTextBox.Text = _marker.Name;

                NoteTextBox.Text = _marker.Note;

                MarkerColorPicker.SelectedColorHex = _marker.Color;
            }
            else
            {
                this.Title = TryFindResource("AddButtonText") as String + " " + this.Title;
                _isEditMode = false;
            }
        }

        private void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!(this.DataContext is ObservableCollection<PlayerMarker> context)) { return; }

            string inputName = NameTextBox.Text;
            string noteText = NoteTextBox.Text;
            string colorText = MarkerColorPicker.SelectedColorHex;

            // Hide error, reset the block
            _blockClose = false;

            // Validate if input information is OK
            if (String.IsNullOrWhiteSpace(inputName))
            {
                // Show error
                _blockClose = true;

                var flyout = new FlyoutHelper(false);
                flyout.TextBlock.Text = TryFindResource("PlayerMarkerNameIsEmptyErrorText") as String;
                flyout.Flyout.ShowAt(NameTextBox);

                return;
            }

            // Check if name already exists
            if (!_isEditMode)   // If we are creating a new item, no need to check old name collision
            {
                var existingItem = context.Where
                    (
                        x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase)
                    ).FirstOrDefault();

                // Name already exists
                if (existingItem != null)
                {
                    _blockClose = true;

                    var errorText = (TryFindResource("PlayerMarkerAlreadyExistsErrorText") as String)
                        .Replace("$", inputName);

                    var flyout = new FlyoutHelper(false);
                    flyout.TextBlock.Text = errorText;
                    flyout.Flyout.ShowAt(NameTextBox);

                    return;
                }

                // New one, add it!!!!!
                var newMarker = new PlayerMarker
                {
                    Name = inputName,
                    Color = colorText,
                    Note = noteText
                };
                context.Add(newMarker);
                this.Hide();
            }
            else
            {
                // Make sure you aren't changing the name to another marker
                var existingItem = context.Where
                    (
                        x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase) &&
                             !x.Name.Equals(_oldName, StringComparison.OrdinalIgnoreCase)
                    ).FirstOrDefault();

                // Name already exists
                if (existingItem != null)
                {
                    _blockClose = true;

                    var errorText = (TryFindResource("PlayerMarkerAlreadyExistsErrorText") as String)
                        .Replace("$", inputName);

                    var flyout = new FlyoutHelper(false);
                    flyout.TextBlock.Text = errorText;
                    flyout.Flyout.ShowAt(NameTextBox);

                    return;
                }

                // New one, add it!!!!!
                _marker.Name = inputName;
                _marker.Color = colorText;
                _marker.Note = noteText;
                this.Hide();
            }
        }

        private void CancelButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _blockClose = false;
            this.Hide();
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (_blockClose)
            {
                args.Cancel = true;
            }
        }
    }
}
