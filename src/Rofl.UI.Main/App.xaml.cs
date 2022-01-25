using Etirps.RiZhi;
using Microsoft.Extensions.Configuration;
using ModernWpf;
using Rofl.Configuration.Models;
using Rofl.Executables.Old;
using Rofl.Files;
using Rofl.Requests;
using Rofl.UI.Main.Models;
using Rofl.UI.Main.Utilities;
using Rofl.UI.Main.Views;
using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Media;

[assembly: SupportedOSPlatform("windows10.0.18362.0")]
namespace Rofl.UI.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private FileManager _files;
        private RequestManager _requests;
        private RiZhi _log;
        private ReplayPlayer _player;
        private ObservableConfiguration _configuration;
        private ExecutableManager _executables;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // load settings
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build()
                .Get<ConfigurationFile>();
            _configuration = new ObservableConfiguration(config);

            CreateCommonObjects();

            // Apply appearence theme
            ApplyThemeSetting();

            // Apply language setting, also loads static data for the language
            LanguageHelper.SetProgramLanguage(_configuration.Language);

#if DEBUG
            _log.Error("Debug mode, writing log");
#endif

            // there is a launch argument
            if (e.Args.Length == 1)
            {
                string selectedFile = e.Args[0];
                // 0 = directly play, 1 = open in replaybook
                if (_configuration.FileAction == FileAction.Play)
                {
                    // Start a blank/invisible window that will host dialogs, otherwise dialogs are invisible
                    var host = new DialogHostWindow();
                    host.Show();
                    await _player.PlayReplay(selectedFile).ConfigureAwait(true);
                    Current.Shutdown();
                }
                else if (_configuration.FileAction == FileAction.Open)
                {
                    var singleWindow = new SingleReplayWindow(_log, _configuration, _requests, _executables, _files, _player)
                    {
                        ReplayFileLocation = selectedFile
                    };
                    singleWindow.Show();
                }
            }
            else
            {
                var mainWindow = new MainWindow(_log, _configuration, _requests, _executables, _files, _player);
                mainWindow.Show();
            }
        }

        private void CreateCommonObjects()
        {
            // Create common objects
            AssemblyName assemblyName = Assembly.GetEntryAssembly()?.GetName();

            _log = new RiZhi()
            {
                FilePrefix = "ReplayBookLog",
                AssemblyName = assemblyName.Name,
                AssemblyVersion = assemblyName.Version.ToString(2)
            };

            try
            {
                _executables = new ExecutableManager(_log);
                _files = new FileManager(_configuration, _log);
                _requests = new RequestManager(_configuration, ApplicationProperties.UserAgent, _log);
                _player = new ReplayPlayer(_files, _configuration, _executables, _log);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                _log.WriteLog();
                throw;
            }
        }

        private void ApplyThemeSetting()
        {
            // Set theme mode
            switch (_configuration.ThemeMode)
            {
                case Theme.SystemAssigned:
                    // null defaults to system assignment
                    ThemeManager.Current.ApplicationTheme = null;
                    break;
                case Theme.Light:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    break;
                case Theme.Dark:
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    break;
            }

            // Set accent color
            ThemeManager.Current.AccentColor = string.IsNullOrEmpty(_configuration.AccentColor)
                ? null
                : (Color)ColorConverter.ConvertFromString(_configuration.AccentColor);
        }

        /// <summary>
        /// Handles writing logs when the program closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_log.ErrorFlag)
            {
                _log.WriteLog();
            }
        }
    }
}
