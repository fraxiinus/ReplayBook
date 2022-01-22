using ModernWpf.Controls;
using Rofl.Configuration.Models;
using Rofl.UI.Main.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for PlayerMarkerWindow.xaml
    /// </summary>
    public partial class PlayerMarkerDialog : ContentDialog
    {
        private readonly PlayerMarkerConfiguration _marker;
        private readonly string _oldName;
        private readonly bool _isEditMode;

        private bool _blockClose;

        public PlayerMarkerDialog()
        {
            _isEditMode = false;
            _blockClose = false;
            InitializeComponent();

            Title = $"{TryFindResource("AddButtonText") as string} {Title}";
            MarkerColorPicker.SelectedColor = Colors.White;
        }

        public PlayerMarkerDialog(PlayerMarkerConfiguration marker)
        {
            InitializeComponent();

            if (marker != null)
            {
                _isEditMode = true;
                _marker = marker;
                _oldName = _marker.Name;

                Title = $"{TryFindResource("EditButtonText") as string} {Title}";
                NameTextBox.Text = _marker.Name;

                NoteTextBox.Text = _marker.Note;

                MarkerColorPicker.SelectedColorHex = _marker.Color;
            }
            else
            {
                Title = $"{TryFindResource("AddButtonText") as string} {Title}";
                _isEditMode = false;
            }
        }

        private void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DataContext is not ObservableCollection<PlayerMarkerConfiguration> context) { return; }

            string inputName = NameTextBox.Text;
            string noteText = NoteTextBox.Text;
            string colorText = MarkerColorPicker.SelectedColorHex;

            // Hide error, reset the block
            _blockClose = false;

            // Validate if input information is OK
            if (string.IsNullOrWhiteSpace(inputName))
            {
                // Show error
                _blockClose = true;

                Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("PlayerMarkerNameIsEmptyErrorText") as string);
                flyout.ShowAt(NameTextBox);

                return;
            }

            // Check if name already exists
            if (!_isEditMode)   // If we are creating a new item, no need to check old name collision
            {
                PlayerMarkerConfiguration existingItem = context.FirstOrDefault(x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase));

                // Name already exists
                if (existingItem != null)
                {
                    _blockClose = true;

                    string errorText = (TryFindResource("PlayerMarkerAlreadyExistsErrorText") as string)
                        .Replace("$", inputName);

                    Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                    flyout.SetFlyoutLabelText(errorText);
                    flyout.ShowAt(NameTextBox);

                    return;
                }

                // New one, add it!!!!!
                var newMarker = new PlayerMarkerConfiguration
                {
                    Name = inputName,
                    Color = colorText,
                    Note = noteText
                };
                context.Add(newMarker);
                Hide();
            }
            else
            {
                // Make sure you aren't changing the name to another marker
                PlayerMarkerConfiguration existingItem = context.FirstOrDefault(x => x.Name.Equals(inputName, StringComparison.OrdinalIgnoreCase)
                                                                    && !x.Name.Equals(_oldName, StringComparison.OrdinalIgnoreCase));

                // Name already exists
                if (existingItem != null)
                {
                    _blockClose = true;

                    string errorText = (TryFindResource("PlayerMarkerAlreadyExistsErrorText") as string)
                        .Replace("$", inputName);

                    Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                    flyout.SetFlyoutLabelText(errorText);
                    flyout.ShowAt(NameTextBox);

                    return;
                }

                // New one, add it!!!!!
                _marker.Name = inputName;
                _marker.Color = colorText;
                _marker.Note = noteText;
                Hide();
            }
        }

        private void CancelButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _blockClose = false;
            Hide();
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
