using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ROFLPlayer
{
    public partial class Form1 : Form
    {

        private string LoLExecFile = ""; // for storing location of league game exe
        private bool LoLFound = false; // used to check if league game exe is set or not
        private string ReplayFile = ""; // for storing location of replay file
        private bool ReplayFound = false; // check if there is a replay set or not
        private List<string> CopiedFiles = new List<string>(); // list of files copied/linked to league game directory

        ///////// data and method used for creating symbolic links
        enum SymbolicLink 
        {
            File = 0,
            Directory = 1
        }

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
        /////////

        ///////// methods for enabling single instance
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == WinMethods.WM_SHOWME) // if message equals to what is sent when a new instance is being started
            {
                ShowMe();
                if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "instance.tmp")) // if the new instance was run using open with dialog
                {
                    string[] instance_data = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "instance.tmp"); // read written arguments from new instance
                    if (SetReplay(instance_data[0]) && LoLFound) // play replay if valid
                    {
                        LoadReplay();
                    }
                }
            }
            base.WndProc(ref m);
        }

        private void ShowMe() // bring myself to the top of all other windows
        {
            bool top = TopMost;
            TopMost = true;
            TopMost = top;
        }
        /////////

        public Form1(string[] args) // Form constructor
        {
            InitializeComponent(); // Initialize form

            if (args.Length == 1) // handle open with dialog
            {
                if (SetReplay(args[0]) && LoLFound) // if argument is valid and league game exe already set, load the replay
                {
                    LoadReplay();
                }
            }
        }

        private void CleanUpAndClose() // handles cleaning up after application
        {
            foreach (string rp in CopiedFiles) // delete every file made in the league game directory
            {
                File.Delete(rp);
            }
            Application.Exit(); // exit
        }

        /*
        private void CreateLink(string linkpath, string targetpath) // debug wrapper to display error msg
        {
            if(!CreateSymbolicLink(linkpath, targetpath, SymbolicLink.File))
            {
                MessageBox.Show("SymLink Fail", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool IsAdministrator() // check if the application is being run as an administrator
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }
        */

        private bool SetReplay(string replaypath) // do all the checks and set replay path
        {
            if (File.Exists(replaypath) && Path.GetExtension(replaypath) == ".rofl") // if the file exists and file extension is .rofl
            {
                ReplayFound = true; // set the flag that the replay is found
                ReplayFile = replaypath; // save the file path
                label2.Text = Path.GetFileName(replaypath); // update the ui
                return true;
            }
            else // error message otherwise
            {
                MessageBox.Show("Replays files must end with .rofl", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SetLoL(string lolpath) // do all the checks for setting league game exe path
        {
            if(File.Exists(lolpath) && Path.GetFileName(lolpath) == @"League of Legends.exe") // if the file exists and is the right name
            {
                LoLFound = true; // set the flag
                LoLExecFile = lolpath; // save the path
                textBoxLoLPath.Text = lolpath; // update the txtbox
                labelValid.Text = "✔️"; // update valid display
                labelValid.ForeColor = Color.Green;
                textBoxLoLPath.Enabled = false; //disable txtbox from edits

                if(RoflSettings.Default.LoLExecLocation != LoLExecFile) // if different from saved data, update the saved data
                {
                    RoflSettings.Default.LoLExecLocation = LoLExecFile;
                    RoflSettings.Default.Save();
                }
                return true;
            }
            else
            {
                LoLFound = false; // undo the flag
                LoLExecFile = ""; //reset the path
                textBoxLoLPath.Text = "Browse for LoL game executable..."; //reset to default
                labelValid.Text = "❗"; // show invalid
                labelValid.ForeColor = Color.Red;
                textBoxLoLPath.Enabled = true; // allow changes to txtbox

                if(!lolpath.Equals("clear") && !lolpath.Equals("Browse for LoL game executable...")) // if the changed text is not something the program does, display error
                {
                    MessageBox.Show("Incorrect LoL game executable!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else // clear saved data 
                {
                    RoflSettings.Default.LoLExecLocation = "";
                    RoflSettings.Default.Save();
                }

                return false;
            }
        }

        private bool PlayButtonCheck() // checks if lol and replay are set, enables or disables play button accordingly
        {
            if(LoLFound && ReplayFound)
            {
                buttonPlay.Enabled = true;
                return true;
            }
            else
            {
                buttonPlay.Enabled = false;
                return false;
            }
        }

        private void LoadReplay() // start a replay
        {
            string newpath = Path.GetDirectoryName(LoLExecFile) + "\\" + Path.GetFileName(ReplayFile); // path that the replay will be located
            try
            {
                if (!File.Exists(newpath)) // do not copy/link if already exists
                {
                    if (!CreateSymbolicLink(newpath, ReplayFile, SymbolicLink.File)) // try creating a symlink first, if fail just copy
                    {
                        File.Copy(ReplayFile, newpath, true);
                    }
                    CopiedFiles.Add(newpath); // add created file to list to delete later
                }
            }
            catch (IOException exc) // issue copying
            {
                MessageBox.Show($"{exc.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); // display error
                label2.Text = "Drag Replays Here"; // update values
                ReplayFound = false;
                PlayButtonCheck();
                return;
            }

            label2.Text = "Playing...";

            var proc = new Process(); // new process
            proc.StartInfo.FileName = LoLExecFile; // set process to lol game
            proc.StartInfo.Arguments = Path.GetFileName(ReplayFile); // set argument to copied replay
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(LoLExecFile); // set working dir
            proc.Start(); // start the replay
            proc.WaitForExit(); // wait here until the replay is over
            label2.Text = Path.GetFileName(ReplayFile); // reset text
        }

        private void textBoxLoLPath_DragDrop(object sender, DragEventArgs e) // hangled dropped data
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                SetLoL(files[0]);
            }
        }

        private void textBoxLoLPath_DragEnter(object sender, DragEventArgs e) // change mouse over cursor
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void textBoxLolPath_DragOver(object sender, DragEventArgs e) // changed mouse over cursor
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

        private void buttonClear_Click(object sender, EventArgs e) // clear button
        {
            SetLoL("clear");
        }

        private void buttonBrowse_Click(object sender, EventArgs e) // lol game browse button
        {
            var FD = new OpenFileDialog();
            FD.Filter = "Executable files (*.exe)|*.exe"; // only show exe files
            if (FD.ShowDialog() == DialogResult.OK)
            {
                SetLoL(FD.FileName);
            }
        }

        private void textBoxLoLPath_TextChanged(object sender, EventArgs e) // for when text is changed
        {
            if (!textBoxLoLPath.Focused)
            {
                SetLoL(textBoxLoLPath.Text);
            }
        }

        private void textBoxLoLPath_KeyDown(object sender, KeyEventArgs e) // handle enter key
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (SetLoL(textBoxLoLPath.Text))
                {
                    this.ActiveControl = buttonRPBrowse; // move focus to browse replay key
                }
            }
        }

        private void splitsplitContainer1_Panel2_DragDrop(object sender, DragEventArgs e) // handle data dropped for replay
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                SetReplay(files[0]);
            }
        }

        private void splitsplitContainer1_Panel2_DragEnter(object sender, DragEventArgs e) // change mouse cursor for replay drag
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void splitsplitContainer1_Panel2_DragOver(object sender, DragEventArgs e) // change mosue cursor for replay drag
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

        private void buttonPlay_Click(object sender, EventArgs e) // play button click
        {
            if(!PlayButtonCheck()) // safety check
            {
                MessageBox.Show("Check if LoL executable or Replay is valid", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // disable all controls during replay
            buttonPlay.Enabled = false;
            buttonRPBrowse.Enabled = false;
            buttonClear.Enabled = false;
            buttonBrowse.Enabled = false;

            LoadReplay();

            //reenable
            buttonPlay.Enabled = true;
            buttonRPBrowse.Enabled = true;
            buttonClear.Enabled = true;
            buttonBrowse.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e) // method when form is loaded
        {
            if(RoflSettings.Default.LoLExecLocation != "") // get saved lol directory
            {
                if (File.Exists(RoflSettings.Default.LoLExecLocation))
                {
                    SetLoL(RoflSettings.Default.LoLExecLocation);
                }
            }

            if(PlayButtonCheck()) // open with dialog
            {
                LoadReplay();
                CleanUpAndClose();
            }
        }

        private void Form1_Close(object sender, EventArgs e) // closing form
        {
            CleanUpAndClose();
        }

        private void buttonRPBrowse_Click(object sender, EventArgs e) // replay browse button
        {
            var FD = new OpenFileDialog();
            FD.Filter = "Replay of Legends files (*.rofl)|*.rofl"; // only show rofl files
            if (FD.ShowDialog() == DialogResult.OK)
            {
                ReplayFile = FD.FileName;
                SetReplay(ReplayFile);
            }
        }

        private void label2_TextChanged(object sender, EventArgs e) // check play button every time a replay is loaded or cleared
        {
            PlayButtonCheck();
        }
    }
}
