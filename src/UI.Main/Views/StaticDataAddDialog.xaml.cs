using Fraxiinus.ReplayBook.StaticData;
using ModernWpf.Controls;
using System;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for StaticDataAddDialog.xaml
    /// </summary>
    public partial class StaticDataAddDialog : ContentDialog
    {
        public StaticDataAddDialog()
        {
            InitializeComponent();

            PatchComboBox.SelectedIndex = 0;
        }

        public string SelectedPatch
        {
            get => PatchComboBox.SelectedItem.ToString();
        }

        private StaticDataManager Context
        {
            get => (DataContext is StaticDataManager context)
                ? context
                : throw new Exception("Invalid data context");
        }

        private async void RefreshPatchesButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPatchesButton.IsEnabled = false;
            PatchComboBox.IsEnabled = false;

            try
            {
                await Context.RefreshPatches();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                ErrorTextBlock.Text = ex.Message;
            }

            RefreshPatchesButton.IsEnabled = true;
            PatchComboBox.IsEnabled = true;
        }
    }
}
