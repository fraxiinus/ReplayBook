using Rofl.Executables.Models;
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
    /// Interaction logic for ExecutableLaunchArgsWindow.xaml
    /// </summary>
    public partial class ExecutableLaunchArgsWindow : Window
    {
        private readonly LeagueExecutable _executable;

        public ExecutableLaunchArgsWindow(LeagueExecutable executable)
        {
            if (executable == null) { throw new ArgumentNullException(nameof(executable)); }

            _executable = executable;
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
