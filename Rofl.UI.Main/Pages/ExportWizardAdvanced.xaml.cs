using ModernWpf.Controls;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardAdvanced.xaml
    /// </summary>
    public partial class ExportWizardAdvanced : ModernWpf.Controls.Page
    {
        public ExportWizardAdvanced()
        {
            InitializeComponent();
        }

        private void ExportWizardAdvanced_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.WindowTitleText = "Export Data Advanced Mode";
        }

        private void AttributeFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }
            if (!(sender is TextBox textbox)) { return; }

            if (string.IsNullOrEmpty(textbox.Text))
            {
                context.AttributesView.Filter -= new FilterEventHandler(AttributeFilter);
                return;
            }

            context.AttributesView.Filter -= new FilterEventHandler(AttributeFilter);
            context.AttributesView.Filter += new FilterEventHandler(AttributeFilter);

            context.AttributesView.View.Refresh();
        }

        private void PlayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }
            if (!(sender is CheckBox checkbox)) { return; }

            // get name of the player we just selected
            string playerName = (((SimpleStackPanel)checkbox.Content).Children[1] as TextBlock).Text;

            // get the player object
            Player player = context.Replay.Players.First(x => x.NAME.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            // loop over all attributes and set preview value
            foreach (ExportAttributeSelectItem attribute in context.Attributes)
            {
                attribute.Value = player.GetType().GetProperty(attribute.Name).GetValue(player)?.ToString() ?? "N/A";
            }

            // regeneratee export preview
            context.ExportPreview = ExportHelper.ConstructExportString(context);
        }

        private void AttributeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.ExportPreview = ExportHelper.ConstructExportString(context);
        }

        private void AttributeFilter(object sender, FilterEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            string filterText = context.AttributeFilterText;

            if (!(e.Item is ExportAttributeSelectItem src))
            {
                e.Accepted = false;
            }
            else if (src != null && !src.Name.Contains(filterText.ToUpper(CultureInfo.InvariantCulture)))// here is FirstName a Property in my YourCollectionItem
            {
                e.Accepted = false;
            }
        }

        private void LoadAttributes()
        {
            if (!(DataContext is ExportDataContext context)) { return; }

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
