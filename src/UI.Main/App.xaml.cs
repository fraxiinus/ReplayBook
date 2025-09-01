using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using Fraxiinus.ReplayBook.Files;
using Fraxiinus.ReplayBook.StaticData;
using Fraxiinus.ReplayBook.UI.Main.Utilities;
using Fraxiinus.ReplayBook.UI.Main.ViewModels;
using Fraxiinus.ReplayBook.UI.Main.Views;
using Microsoft.Extensions.Configuration;
using ModernWpf;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

[assembly: SupportedOSPlatform("windows10.0.18362.0")]
namespace Fraxiinus.ReplayBook.UI.Main;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private FileManager _files;
    private StaticDataManager _staticDataManager;
    private RiZhi _log;
    private ReplayPlayer _player;
    private ObservableConfiguration _configuration;
    private ExecutableManager _executables;

    /// <summary>
    /// Application entry point
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        await StartApplication(e);
    }

    private static void ShouldRestart(bool restartApplication)
    {
        if (restartApplication)
        {
            var currentExecutablePath = Environment.ProcessPath;
            Current.Shutdown();
            Process.Start(currentExecutablePath);
        }
    }

    private async Task StartApplication(StartupEventArgs e)
    {
        // load settings
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build()
            .Get<ConfigurationFile>();
        _configuration = new ObservableConfiguration(config);

        // Apply language setting, also loads static data for the language
        LanguageHelper.SetProgramLanguage(_configuration.Language);

        // Short circuit - Disable Vanguard And Exit
        if (e.Args.Length == 2 && string.Equals(e.Args[0], "disable"))
        {
            AssemblyName assemblyName = Assembly.GetEntryAssembly()?.GetName();

            // Create new logger just for this action
            _log = new RiZhi()
            {
                FilePrefix = "ReplayBookLog_DisableVanguard",
                AssemblyName = assemblyName.Name,
                AssemblyVersion = assemblyName.Version.ToString(2)
            };

            // Start a blank/invisible window that will host dialogs, otherwise dialogs are invisible
            var host = new DialogHostWindow();
            host.Show();
            var (success, message) = await VanguardServiceHelper.TryStopVanguardAsync(_log);

            if (!success)
            {
                _log.Error($"{message}");
            }

            Current.Shutdown();
            return;
        }

        // create underlying objects
        await CreateCommonObjects();

        // Apply appearance & theme
        ApplyThemeSetting();

#if DEBUG
        _log.Error("Debug mode, writing log");
#endif

        // Directly open with a replay file
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
                return;
            }
            else if (_configuration.FileAction == FileAction.Open)
            {
                var singleWindow = new SingleReplayWindow(_log, _configuration, _staticDataManager, _executables, _files, _player)
                {
                    ReplayFileLocation = selectedFile
                };
                // Capture if restart is needed
                singleWindow.Closed += (s, eventArgs) =>
                {
                    if (singleWindow.DataContext is MainWindowViewModel viewModel)
                    {
                        ShouldRestart(viewModel.RestartOnClose);
                    }
                };
                singleWindow.Show();
            }
        }
        // Start ReplayBook as normal
        else
        {
            var mainWindow = new MainWindow(_log, _configuration, _staticDataManager, _executables, _files, _player);
            mainWindow.Closed += (s, eventArgs) =>
            {
                if (mainWindow.DataContext is MainWindowViewModel viewModel)
                {
                    ShouldRestart(viewModel.RestartOnClose);
                }
            };
            mainWindow.RestoreSavedWindowState();
            mainWindow.Show();
        }
    }

    private async Task CreateCommonObjects()
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
            _staticDataManager = new StaticDataManager(_configuration, ApplicationProperties.UserAgent, _log);
            await _staticDataManager.LoadIndexAsync();

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
