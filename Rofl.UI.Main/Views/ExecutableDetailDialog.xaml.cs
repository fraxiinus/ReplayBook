using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;
using Rofl.Executables;
using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.UI.Main.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rofl.UI.Main.Views
{
    /// <summary>
    /// Interaction logic for ExecutableDetailWindow.xaml
    /// </summary>
    public partial class ExecutableDetailDialog : ContentDialog
    {
        private LeagueExecutable _executable;
        private readonly bool _isEditMode;
        private bool _blockClose;
        private readonly IEnumerable<string> LocaleNames;

        public ExecutableDetailDialog()
        {
            // load locales into drop down, skip parentheses for custom option
            LocaleNames = Enum.GetNames(typeof(LeagueLocale))
                .Select(x => x + (x.StartsWith(LeagueLocale.Custom.ToString(), StringComparison.OrdinalIgnoreCase) ? "" : " (" + ExeTools.GetLocaleCode(x) + ")"))
                .OrderBy(x => x == LeagueLocale.Custom.ToString())
                .ThenBy(x => x);

            InitializeComponent();
            _isEditMode = false;

            Title = TryFindResource("AddButtonText") as string + " " + Title;
        }

        public ExecutableDetailDialog(LeagueExecutable executable)
        {
            if (executable == null) { throw new ArgumentNullException(nameof(executable)); }

            // load locales into drop down, skip parentheses for custom option
            LocaleNames = Enum.GetNames(typeof(LeagueLocale))
                .Select(x => x + (x.StartsWith(LeagueLocale.Custom.ToString(), StringComparison.OrdinalIgnoreCase) ? "" : " (" + ExeTools.GetLocaleCode(x) + ")"))
                .OrderBy(x => x == LeagueLocale.Custom.ToString())
                .ThenBy(x => x);

            InitializeComponent();

            LoadLeagueExecutable(executable);
            _isEditMode = true;
            Title = TryFindResource("EditButtonText") as string + " " + Title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

            LocaleComboBox.ItemsSource = LocaleNames;
        }

        private void LoadLeagueExecutable(LeagueExecutable executable)
        {
            _executable = executable ?? throw new ArgumentNullException(nameof(executable));

            TargetTextBox.Text = executable.TargetPath;
            NameTextBox.Text = executable.Name;

            LocaleComboBox.SelectedItem = executable.Locale == LeagueLocale.Custom
                ? LocaleNames.Last()
                : LocaleNames.First(x => x.Split('(', ')')[1] == ExeTools.GetLocaleCode(executable.Locale));

            LaunchArgsTextBox.Text = PrettifyLaunchArgs(executable.LaunchArguments);
            CustomLocaleTextBox.Text = executable.CustomLocale;
        }

        private void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!(DataContext is ExecutableManager context)) { return; }

            // Reset error
            _blockClose = false;

            if (_executable == null)
            {
                _blockClose = true;

                // show fly out
                Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as string);
                flyout.ShowAt(TargetTextBox);

                return;
            }

            try
            {
                _executable.Name = NameTextBox.Text;
                _executable.TargetPath = TargetTextBox.Text;

                string locale = LocaleComboBox.SelectedItem as string;

                if ((LocaleComboBox.SelectedItem as string) == LeagueLocale.Custom.ToString())
                {
                    _executable.Locale = LeagueLocale.Custom;
                    _executable.CustomLocale = CustomLocaleTextBox.Text;
                }
                else
                {
                    _executable.Locale = ExeTools.GetLocaleEnum((LocaleComboBox.SelectedItem as string).Split('(', ')')[1]);
                }

                if (_isEditMode)
                {
                    Hide();
                }
                else
                {
                    context.AddExecutable(_executable);
                }

                Hide();
            }
            catch (Exception)
            {
                _blockClose = true;

                // show fly out
                Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("ExecutableSaveNullText") as string);
                flyout.ShowAt(TargetTextBox);
            }
        }

        private void CancelButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _blockClose = false;
            Hide();
        }

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            _blockClose = false;

            string initialDirectory = TargetTextBox.Text;
            initialDirectory = string.IsNullOrEmpty(initialDirectory)
                ? Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))
                : Path.GetDirectoryName(initialDirectory);

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.Title = TryFindResource("ExecutableSelectDialogText") as string;
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
                    string selectedExe = folderDialog.FileName;

                    if (ExeTools.CheckExecutableFile(selectedExe))
                    {
                        LeagueExecutable newExe = null;

                        try
                        {
                            newExe = ExeTools.CreateNewLeagueExecutable(selectedExe);

                            try
                            {
                                newExe.Locale = ExeTools.DetectExecutableLocale(selectedExe);
                            }
                            catch (Exception)
                            {
                                newExe.Locale = LeagueLocale.EnglishUS;
                                // do not stop operation
                            }

                            LoadLeagueExecutable(newExe);
                        }
                        catch (Exception)
                        {
                            _blockClose = true;

                            // show fly out
                            Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                            flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as string);
                            flyout.ShowAt(TargetButton);
                        }
                    }
                    else
                    {
                        _blockClose = true;

                        // show fly out
                        Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                        flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as string);
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
                Flyout flyout = FlyoutHelper.CreateFlyout(includeButton: false);
                flyout.SetFlyoutLabelText(TryFindResource("ExecutableSelectInvalidErrorText") as string);
                flyout.ShowAt(EditLaunchArgsButton);

                return;
            }

            Window parent = Window.GetWindow(this);

            ExecutableLaunchArgsWindow editDialog = new ExecutableLaunchArgsWindow(_executable)
            {
                Top = parent.Top + 50,
                Left = parent.Left + 50,
                Owner = parent
            };

            editDialog.ShowDialog();
            LaunchArgsTextBox.Text = PrettifyLaunchArgs(_executable.LaunchArguments);
        }

        private static string PrettifyLaunchArgs(string args)
        {
            IEnumerable<string> individual = args.Split('\"').Where(x => !string.IsNullOrWhiteSpace(x));
            return string.Join("\n", individual);
        }

        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (_blockClose)
            {
                args.Cancel = true;
            }
        }

        private void LocaleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // show custom locale controls when option is selected
            CustomLocaleContainer.Visibility = (LocaleComboBox.SelectedItem as string) == LeagueLocale.Custom.ToString()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
