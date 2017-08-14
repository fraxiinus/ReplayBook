using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace ROFLPlayer
{
    public partial class Form1 : Form
    {

        private string LoLExecFile = "";
        private bool LoLFound = false;
        private string ReplayFile = "";
        private bool ReplayFound = false;
        private List<string> CopiedFiles = new List<string>();

        public Form1(string[] args)
        {
            InitializeComponent();
            if(args.Length == 1)
            {
                ReplayFile = args[0];
                ReplayFound = true;
            }
        }
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == WinMethods.WM_SHOWME)
            {
                ShowMe();
            }
            base.WndProc(ref m);
        }

        private void ShowMe()
        {

            bool top = TopMost;

            TopMost = true;

            TopMost = top;
        }

        private void textBoxLoLPath_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                textBoxLoLPath.Text = files[0];
            }
        }

        private void textBoxLoLPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void textBoxLolPath_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxLoLPath.Text = "Browse for LoL game executable...";
            LoLFound = false;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var FD = new OpenFileDialog();
            FD.Filter = "Executable files (*.exe)|*.exe";
            if (FD.ShowDialog() == DialogResult.OK)
            {
                textBoxLoLPath.Text = FD.FileName;
            }
        }

        private void textBoxLoLPath_TextChanged(object sender, EventArgs e)
        {
            if(Regex.IsMatch(textBoxLoLPath.Text, @"League of Legends.exe"))
            {
                labelValid.Text = "Looks Good";
                LoLFound = true;
                labelValid.ForeColor = Color.Green;
                LoLExecFile = textBoxLoLPath.Text;
                textBoxLoLPath.Enabled = false;
                Settings1.Default.LoLExecLocation = LoLExecFile;
                Settings1.Default.Save();
                if (ReplayFile != "")
                {
                    buttonPlay.Enabled = true;
                }
            }
            else
            {
                labelValid.Text = "Find Exec";
                labelValid.ForeColor = Color.Red;
                LoLExecFile = "";
                buttonPlay.Enabled = false;
                textBoxLoLPath.Enabled = true;
                LoLFound = false;
            }
        }

        private void splitsplitContainer1_Panel2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                if(Path.GetExtension(files[0]) != ".rofl")
                {
                    MessageBox.Show("Invalid file type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ReplayFile = "";
                    ReplayFound = false;
                    buttonPlay.Enabled = false;
                }
                else
                {
                    ReplayFile = files[0];
                    label2.Text = Path.GetFileName(ReplayFile);
                    if(LoLExecFile != "")
                    {
                        buttonPlay.Enabled = true;
                    }
                    ReplayFound = true;
                }
            }
        }

        private void splitsplitContainer1_Panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void splitsplitContainer1_Panel2_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if(!ReplayFound || !LoLFound)
            {
                MessageBox.Show("Check if LoL executable or Replay is valid", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            buttonPlay.Enabled = false;
            buttonRPBrowse.Enabled = false;
            buttonClear.Enabled = false;
            buttonBrowse.Enabled = false;
            loadReplay();
            buttonPlay.Enabled = true;
            buttonRPBrowse.Enabled = true;
            buttonClear.Enabled = true;
            buttonBrowse.Enabled = true;
            //label2.Text = "Drag Replays Here";
        }

        private void loadReplay()
        {
            label2.Text = "Copying...";
            string newpath = Path.GetDirectoryName(LoLExecFile) + "\\" + Path.GetFileName(ReplayFile);
            try
            {
                if (!File.Exists(newpath))
                {
                    File.Copy(ReplayFile, newpath, true);
                    CopiedFiles.Add(newpath);
                }
            }
            catch (IOException exc)
            {
                MessageBox.Show($"{exc.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label2.Text = "Drag Replays Here";
                return;
            }
            label2.Text = "Playing...";
            var proc = new Process();

            proc.StartInfo.FileName = LoLExecFile;
            proc.StartInfo.Arguments = Path.GetFileName(newpath);
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(LoLExecFile);
            proc.Start();
            proc.WaitForExit();
            label2.Text = Path.GetFileName(ReplayFile);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(Settings1.Default.LoLExecLocation != "")
            {
                if (File.Exists(Settings1.Default.LoLExecLocation))
                {
                    textBoxLoLPath.Text = Settings1.Default.LoLExecLocation;
                }
            }
            if(ReplayFound && LoLFound)
            {
                loadReplay();
                Application.Exit();
            }
        }

        private void Form1_Close(object sender, EventArgs e)
        {
            foreach (string rp in CopiedFiles)
            {
                File.Delete(rp);
            }
        }

        private void buttonRPBrowse_Click(object sender, EventArgs e)
        {
            var FD = new OpenFileDialog();
            FD.Filter = "Replay of Legends files (*.rofl)|*.rofl";
            if (FD.ShowDialog() == DialogResult.OK)
            {
                ReplayFile = FD.FileName;
                label2.Text = Path.GetFileName(ReplayFile);
                if (LoLExecFile != "")
                {
                    buttonPlay.Enabled = true;
                }
            }
        }

    }
}
