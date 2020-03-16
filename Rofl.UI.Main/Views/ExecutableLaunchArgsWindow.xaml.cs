using Rofl.Executables.Models;
using System;
using System.Windows;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExecutableLaunchArgsWindow.xaml
    /// </summary>
    public partial class ExecutableLaunchArgsWindow : Window
    {
        private readonly LeagueExecutable _executable;

        public ExecutableLaunchArgsWindow(LeagueExecutable executable)
        {
            _executable = executable ?? throw new ArgumentNullException(nameof(executable));

            InitializeComponent();

            LaunchArgsBox.Text = _executable.LaunchArguments;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _executable.LaunchArguments = LaunchArgsBox.Text;
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
