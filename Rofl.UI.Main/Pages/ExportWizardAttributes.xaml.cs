using ModernWpf.Controls;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for ExportWizardAttributes.xaml
    /// </summary>
    public partial class ExportWizardAttributes : ModernWpf.Controls.Page
    {
        public ExportWizardAttributes()
        {
            InitializeComponent();
        }

        private void ExportWizardAttributes_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            // get first checked player
            string playerName = context.Players.FirstOrDefault(x => x.Checked)?.PlayerPreview.PlayerName;
            Player player = context.Replay.Players.FirstOrDefault(x => x.NAME.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            if (player != null)
            {
                // loop over all attributes and set preview value
                foreach (ExportAttributeSelectItem attribute in context.Attributes)
                {
                    attribute.Value = player.GetType().GetProperty(attribute.Name).GetValue(player)?.ToString() ?? "N/A";
                }
            }
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            context.ContentFrame.GoBack();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            _ = context.ContentFrame.Navigate(typeof(ExportWizardFinish));
        }

        private void SelectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
            {
                attributeSelect.Checked = true;
            }
        }

        private void DeselectAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is ExportDataContext context)) { return; }

            foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
            {
                attributeSelect.Checked = true;
            }
        }
    }
}
