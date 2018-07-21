using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROFLPlayer.Lib
{
    public class MainWindowManager
    {
        public static void Load(Form form)
        {
            form.Controls.Find("GeneralGameTextBox", true)[0].Text = RoflSettings.Default.LoLExecLocation;
        }

        public static void CloseWindow(Form form)
        {
            // Save and Exit form
            // Save Executable location
            RoflSettings.Default.LoLExecLocation = form.Controls.Find("GeneralGameTextBox", true)[0].Text;
            // Save double click launch option
            RoflSettings.Default.StartupMode = ((ComboBox)form.Controls.Find("GeneralLaunchComboBox", true)[0]).SelectedIndex;
            RoflSettings.Default.Username = form.Controls.Find("GeneralUsernameTextBox", true)[0].Text;
            RoflSettings.Default.Region = ((ComboBox)form.Controls.Find("GeneralRegionComboBox", true)[0]).Text;
            RoflSettings.Default.Save();
            Environment.Exit(1);
        }
    }
}
