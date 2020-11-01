﻿using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;
using Rofl.Executables;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.UI.Main.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExecutableDetailWindow.xaml
    /// </summary>
    public partial class ExecutableDetailDialog : ContentDialog
    {
        private LeagueExecutable _executable;
        private readonly bool _isEditMode;
        private bool _blockClose = false;

        public ExecutableDetailDialog()
        {
            InitializeComponent();
            _isEditMode = false;

            this.Title = TryFindResource("AddButtonText") as String + " " + this.Title;
        }

        public ExecutableDetailDialog(LeagueExecutable executable)
        {
            if (executable == null) { throw new ArgumentNullException(nameof(executable)); }

            InitializeComponent();

            LoadLeagueExecutable(executable);
            _isEditMode = true;
            this.Title = TryFindResource("EditButtonText") as String + " " + this.Title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var allLocales = Enum.GetNames(typeof(LeagueLocale)).Select(x => x + " (" + ExeTools.GetLocaleCode(x) + ")");

            this.LocaleComboBox.ItemsSource = allLocales;
        }

        private void LoadLeagueExecutable(LeagueExecutable executable)
        {
            _executable = executable ?? throw new ArgumentNullException(nameof(executable));

            TargetTextBox.Text = executable.TargetPath;
            NameTextBox.Text = executable.Name;
            LocaleComboBox.SelectedIndex = (int) executable.Locale;
            LaunchArgsTextBox.Text = PrettifyLaunchArgs(executable.LaunchArguments);
            //ModifiedDateTextBox.Text = executable.ModifiedDate.ToString("o", CultureInfo.InvariantCulture);
            //StartPathTextBox.Text = executable.StartFolder;
            //PatchTextBox.Text = executable.PatchNumber;
        }

        private void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!(this.DataContext is ExecutableManager context)) { return; }

            // Reset error
            _blockClose = false;

            if (_executable == null)
            {
                _blockClose = true;

                // show fly out
                var flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as String);
                flyout.ShowAt(TargetTextBox);
                
                return;
            }

            try
            {
                _executable.Name = NameTextBox.Text;
                _executable.TargetPath = TargetTextBox.Text;
                _executable.Locale = (LeagueLocale) LocaleComboBox.SelectedIndex;

                if (_isEditMode)
                {
                    this.Hide();
                }
                else
                {
                    context.AddExecutable(_executable);
                }
                
                this.Hide();
            }
            catch (Exception)
            {
                _blockClose = true;

                // show fly out
                var flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("ExecutableSaveNullText") as String);
                flyout.ShowAt(TargetTextBox);
            }
        }

        private void CancelButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _blockClose = false;
            this.Hide();
        }

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            _blockClose = false;

            var initialDirectory = TargetTextBox.Text;
            if (String.IsNullOrEmpty(initialDirectory))
            {
                initialDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
            }
            else
            {
                initialDirectory = Path.GetDirectoryName(initialDirectory);
            }
            
            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("ExecutableSelectDialogText") as String;
                folderDialog.IsFolderPicker = false;
                folderDialog.AddToMostRecentlyUsedList = false;
                folderDialog.AllowNonFileSystemItems = false;
                folderDialog.EnsureFileExists = true;
                folderDialog.EnsurePathExists = true;
                folderDialog.EnsureReadOnly = false;
                folderDialog.EnsureValidNames = true;
                folderDialog.Multiselect = false;
                folderDialog.ShowPlacesList = true;

                folderDialog.InitialDirectory = initialDirectory;
                folderDialog.DefaultDirectory = initialDirectory;

                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var selectedExe = folderDialog.FileName;

                    if (ExeTools.CheckExecutableFile(selectedExe))
                    {
                        var newExe = ExeTools.CreateNewLeagueExecutable(selectedExe);
                        LoadLeagueExecutable(newExe);
                    }
                    else
                    {
                        _blockClose = true;

                        // show fly out
                        var flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                        flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as String);
                        flyout.ShowAt(TargetButton);
                    }
                }
            }
        }

        private void EditLaunchArgsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_executable == null)
            {
                // show fly out
                var flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as String);
                flyout.ShowAt(EditLaunchArgsButton);

                return;
            }

            var parent = Window.GetWindow(this);

            var editDialog = new ExecutableLaunchArgsWindow(_executable)
            {
                Top = parent.Top + 50,
                Left = parent.Left + 50,
                Owner = parent
            };

            editDialog.ShowDialog();
            LaunchArgsTextBox.Text = PrettifyLaunchArgs(_executable.LaunchArguments);
        }

        private string PrettifyLaunchArgs(string args)
        {
            var individual = args.Split('\"').Where(x => !String.IsNullOrWhiteSpace(x));
            return String.Join("\n", individual);
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (_blockClose)
            {
                args.Cancel = true;
            }
        }
    }
}
