using System;
using System.Windows.Forms;
using ROFLPlayer.Lib;

namespace ROFLPlayer
{
    public partial class DetailsForm : Form
    {
        public DetailsForm()
        {
            InitializeComponent();
            MainWindowManager.Load(this);
        }

        private void MainOkButton_Click(object sender, EventArgs e)
        {
            MainWindowManager.CloseWindow(this);
        }

        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
