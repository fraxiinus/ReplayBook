using Fraxiinus.ReplayBook.Reader.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using System;
using System.Linq;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExportDataWindow.xaml
    /// </summary>
    public partial class ExportDataWindow : Window
    {
        private ExportDataContext Context
        {
            get => (DataContext is ExportDataContext context)
                ? context
                : throw new Exception("Invalid data context");
        }

        public ExportDataWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load preview players into list
            Context.Players = Context.ReplayPreview.BluePreviewPlayers
                .Union(Context.ReplayPreview.RedPreviewPlayers)
                .Select(x => new ExportPlayerSelectItem
                {
                    Checked = false,
                    PlayerPreview = x
                })
                .ToList();

            _ = Context.ContentFrame.Navigate(typeof(Pages.ExportWizardLanding));

            LoadAttributes();
        }

        private void LoadAttributes()
        {
            // do not reload attributes list if it has been loaded before (when navigating back to this page for example)
            if (Context.Attributes is null)
            {
                Context.Attributes = typeof(Player)
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