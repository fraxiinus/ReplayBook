using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities;

public class VanguardServiceHelper
{
    public const string VANGUARD_SERVICE_NAME = "vgk";

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

    public static async Task<(bool success, Exception message)> TryStopVanguardAsync()
    {
        using var serviceController = TryGetVanguardService();
        try
        {
            var vanguardStopTask = Task.Run(() => serviceController.Stop());
            await WaitForStatusAsync(serviceController, ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }

    public static async Task WaitForStatusAsync(ServiceController controller, ServiceControllerStatus desiredStatus, TimeSpan timeout)
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
        var servicesList = ServiceController.GetServices();
        return servicesList.Where(x => x.ServiceName == VANGUARD_SERVICE_NAME).FirstOrDefault();
    }
}
