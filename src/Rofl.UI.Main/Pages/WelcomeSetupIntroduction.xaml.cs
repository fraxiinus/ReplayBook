using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Pages
{
    /// <summary>
    /// Interaction logic for WelcomeSetupIntroduction.xaml
    /// </summary>
    public partial class WelcomeSetupIntroduction : ModernWpf.Controls.Page, IWelcomePage
    {
        private WelcomeSetupDataContext Context
        {
            get => (DataContext is WelcomeSetupDataContext context)
                ? context
                : throw new Exception("Invalid data context");
        }

        public WelcomeSetupIntroduction()
        {
            InitializeComponent();

            // load combo box
            LanguageComboBox.ItemsSource = LanguageHelper.GetFriendlyLanguageNames();
        }

        public string GetTitle()
        {
            return (string)TryFindResource("WswIntroFrameTitle");
        }

        public Type GetNextPage()
        {
            return typeof(WelcomeSetupExecutables);
        }

        public Type GetPreviousPage()
        {
            throw new NotSupportedException();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {           
            // select initial language after page is loaded
            LanguageComboBox.SelectedIndex = (int)Context.Language;
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // selection change might trigger when datacontext is not loaded
            try
            {
                LanguageHelper.SetProgramLanguage(Context.Language);
            }
            catch (Exception)
            {
                // do nothing
            }
        }
    }
}
