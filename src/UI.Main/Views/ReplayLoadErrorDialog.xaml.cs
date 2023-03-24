using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.UI.Main.Models;
using ModernWpf.Controls;
using System.Windows;
using System.Windows.Documents;

namespace Fraxiinus.ReplayBook.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ErrorDisplayDialog.xaml
    /// </summary>
    public partial class ReplayLoadErrorDialog : ContentDialog
    {
        public ReplayLoadErrorDialog()
        {
            InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is not StatusBar context) { return; }

            var errorDetails = new FlowDocument();
            foreach (FileErrorResult error in context.Errors)
            {
                var filePath = new Bold(new Run(error.FilePath + "\n"));

                var errorParagraph = new Paragraph();
                errorParagraph.Inlines.Add(filePath);
                errorParagraph.Inlines.Add(new Run(error.Exception.Message));
                errorDetails.Blocks.Add(errorParagraph);
            }
            ErrorDetailsTextBox.Document = errorDetails;
        }
    }
}
