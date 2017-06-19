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
        private string ReplayFile = "";

        public Form1()
        {
            InitializeComponent();
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
                labelValid.Text = "O";
                labelValid.ForeColor = Color.Green;
                LoLExecFile = textBoxLoLPath.Text;
                Settings1.Default.LoLExecLocation = LoLExecFile;
                Settings1.Default.Save();
                if (ReplayFile != "")
                {
                    buttonPlay.Enabled = true;
                }
            }
            else
            {
                labelValid.Text = "X";
                labelValid.ForeColor = Color.Red;
                LoLExecFile = "";
                buttonPlay.Enabled = false;
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
            buttonPlay.Enabled = false;
            label2.Text = "Copying...";
            string newpath = Path.GetDirectoryName(LoLExecFile) + "\\" + Path.GetFileName(ReplayFile);
            File.Copy(ReplayFile, newpath, true);
            label2.Text = "Playing...";
            var proc = new Process();

            proc.StartInfo.FileName = LoLExecFile;
            proc.StartInfo.Arguments = Path.GetFileName(newpath);
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(LoLExecFile);
            proc.Start();
            while (!proc.HasExited) { }
            label2.Text = "Drag Replays Here";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(Settings1.Default.LoLExecLocation != "")
            {
                textBoxLoLPath.Text = Settings1.Default.LoLExecLocation;
            }
        }
    }
}
