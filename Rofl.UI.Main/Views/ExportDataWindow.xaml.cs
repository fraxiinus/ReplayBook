using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExportDataWindow.xaml
    /// </summary>
    public partial class ExportDataWindow : Window
    {
        public ExportDataWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // load preview players into list
            context.Players = context.ReplayPreview.BluePreviewPlayers
                .Union(context.ReplayPreview.RedPreviewPlayers)
                .Select(x => new ExportPlayerSelectItem
                {
                    Checked = false,
                    PlayerPreview = x
                })
                .ToList();

            _ = context.ContentFrame.Navigate(typeof(Pages.ExportWizardLanding));

            LoadAttributes();
        }

        private void LoadAttributes()
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // do not reload attributes list if it has been loaded before (when navigating back to this page for example)
            if (context.Attributes is null)
            {
                context.Attributes = typeof(Player)
                    .GetProperties()
                    .Where(x => !x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) // ignore internal properties
                    .Where(x => !x.Name.Equals("PlayerID", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.Name)
                    .Select(x => new ExportAttributeSelectItem
                    {
                        Checked = false,
                        Name = x.Name,
                        Value = "N/A"
                    })
                    .ToList();
            }
        }
    }
}