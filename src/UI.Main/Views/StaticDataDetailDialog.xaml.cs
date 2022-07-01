using Fraxiinus.ReplayBook.StaticData.Models;
using Fraxiinus.ReplayBook.UI.Main.Converters;
using Fraxiinus.ReplayBook.UI.Main.Models;
using ModernWpf.Controls;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for StaticDataDetailDialog.xaml
    /// </summary>
    public partial class StaticDataDetailDialog : ContentDialog
    {
        public StaticDataDetailDialog()
        {
            InitializeComponent();
        }

        private ObservableBundle Context
        {
            get => (DataContext is ObservableBundle context)
                ? context
                : throw new Exception("Invalid data context");
        }

        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is not SettingsWindow settingsWindow) { return; }

            Title = (Title as string).Replace("$", Context.Patch);

            var totalSize = await (settingsWindow.DataContext as SettingsWindowDataContext)
                .StaticData.CalculateDiskUsage(Context.Patch);

            var readableSizeConverter = new FormatKbSizeConverter();
            SizeValue.Text = (string)readableSizeConverter.Convert(totalSize, null, null, CultureInfo.InvariantCulture);

            ChampionLanguageValue.Text = string.Join(", ", Context.ChampionDataFiles.Select(x => x.language));
            ItemLanguageValue.Text = string.Join(", ", Context.ItemDataFiles.Select(x => x.language));
            RuneLanguageValue.Text = string.Join(", ", Context.RuneDataFiles.Select(x => x.language));
        }
    }
}
