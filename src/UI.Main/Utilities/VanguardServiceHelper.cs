using Etirps.RiZhi;
using Fraxiinus.ReplayBook.UI.Main.Views;
using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities;

public class VanguardServiceHelper
{
    public const string VANGUARD_SERVICE_NAME = "vgk";

    public static async Task<(bool success, Exception message)> StartPrivilegedProcess()
    {
        var replayBookExecutablePath = Environment.ProcessPath;
        var replayBookWorkingDirectory = Path.GetDirectoryName(replayBookExecutablePath);
        ProcessStartInfo processStartInfo = new()
        {
            FileName = replayBookExecutablePath,
            UseShellExecute = true,
            Verb = "runas",
            Arguments = "disable vanguard",
            WorkingDirectory = replayBookWorkingDirectory,
        };

        using var serviceController = TryGetVanguardService();
        var process = Process.Start(processStartInfo);

        // Wait and see if Vanguard has been stopped
        try
        {
            await WaitForStatusAsync(serviceController, ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }

    public static async Task<(bool success, Exception message)> TryStopVanguardAsync(RiZhi log)
    {
        if (!IsAdministrator())
        {
            // Not an administrator, cannot stop vanguard
            var ex = new InvalidOperationException("Requires Elevated privileges to disable Vanguard");
            log.Error($"{ex}");
            await ShowPermissionsErrorDialog();
            return (false, ex);
        }

        log.Information("Getting vanguard service...");
        using var serviceController = TryGetVanguardService();
        try
        {
            log.Information("Attempting to stop service...");
            var vanguardStopTask = Task.Run(() => serviceController.Stop());
            await WaitForStatusAsync(serviceController, ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            return (true, null);
        }
        catch (Exception ex)
        {
            log.Error($"{ex}");
            return (false, ex);
        }
    }

    public static bool IsVanguardRunning()
    {
        using var serviceController = TryGetVanguardService();
        // If service controller is null, then vanguard does not exist
        if (serviceController != null)
        {
            return serviceController.Status != ServiceControllerStatus.Stopped;
        }
        else
        {
            return false;
        }
    }

    private static async Task WaitForStatusAsync(ServiceController controller, ServiceControllerStatus desiredStatus, TimeSpan timeout)
    {
        var utcNow = DateTime.UtcNow;
        controller.Refresh();
        while (controller.Status != desiredStatus)
        {
            if (DateTime.UtcNow - utcNow > timeout)
            {
                throw new System.ServiceProcess.TimeoutException($"Failed to wait for '{controller.ServiceName}' to change status to '{desiredStatus}'.");
            }
            await Task.Delay(250)
                .ConfigureAwait(false);
            controller.Refresh();
        }
    }

    private static ServiceController TryGetVanguardService()
    {
        // Vanguard is a kernel device driver
        var servicesList = ServiceController.GetDevices();
        return servicesList.Where(x => x.ServiceName == VANGUARD_SERVICE_NAME).FirstOrDefault();
    }

    private static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static async Task ShowPermissionsErrorDialog()
    {
        // Creating content dialog
        ContentDialog dialog = ContentDialogHelper.CreateContentDialog(
            title: App.Current.TryFindResource("Main__VanguardPermissionsError__Title") as string,
            description: (App.Current.TryFindResource("Main__VanguardPermissionsError__Body") as string),
            primaryButtonText: App.Current.TryFindResource("OKButtonText") as string);

        // Make background overlay transparent when in the dialog host window,
        // making the dialog appear seamlessly
        if (App.Current.MainWindow is DialogHostWindow)
        {
            dialog.SetBackgroundSmokeColor(Brushes.Transparent);
        }

        _ = await dialog.ShowAsync(ContentDialogPlacement.Popup).ConfigureAwait(true);
    }
}
