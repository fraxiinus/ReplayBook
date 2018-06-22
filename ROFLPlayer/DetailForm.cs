using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ROFLPlayer.Lib;

namespace ROFLPlayer
{
    public partial class DetailForm : Form
    {
        private string replaypath = "";

        public DetailForm(string replayPath)
        {
            replaypath = replayPath;
            InitializeComponent();
            if (LeagueManager.CheckReplayFile(replayPath))
            {
                
            }
            else
            {
                MessageBox.Show("File is not a valid replay.", "Invalid Replay File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private void MainCancelButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void MainOkButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LeagueManager.DumpJSON(replaypath);
        }
    }
}
