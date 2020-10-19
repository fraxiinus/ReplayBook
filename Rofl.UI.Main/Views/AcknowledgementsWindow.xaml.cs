using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for AcknowledgementsWindow.xaml
    /// </summary>
    public partial class AcknowledgementsWindow : Window
    {
        public int AcknowledgementsIndex { get; set; }

        public List<string> Titles { get; private set; }

        public AcknowledgementsWindow()
        {
            InitializeComponent();
            Titles = new List<string>();

            // Count how many times "AckTitle" shows up in the resource dictionary
            var keys = Application.Current.Resources.MergedDictionaries[4].Keys.OfType<string>()
                .Where(x => x.StartsWith("AckTitle", StringComparison.OrdinalIgnoreCase))
                .ToList();

            keys.Sort();
            keys.Reverse();

            // Load titles to list
            foreach (var key in keys)
            {
                Titles.Add(FindResource(key) as string);
            }
            Titles.Reverse();

            AcknowledgementsIndex = 0;
            AcknowledgementsListBox.ItemsSource = Titles;

            AcknowledgementsListBox.SelectedIndex = 0;
        }

        private void AcknowledgementsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = AcknowledgementsListBox.SelectedIndex;
            TitleText.Text = FindResource($"AckTitle{index + 1}") as string;
            AuthorText.Text = FindResource($"AckAuthor{index + 1}") as string;
            HyperlinkButton.NavigateUri = new Uri(FindResource($"AckLink{index + 1}") as string);
            HyperlinkText.Text = FindResource($"AckLink{index + 1}") as string;
            //LinkText.Text = FindResource($"AckLink{index + 1}") as string;
            LicenseText.Text = FindResource($"AckLicense{index + 1}") as string;
        }
    }
}
