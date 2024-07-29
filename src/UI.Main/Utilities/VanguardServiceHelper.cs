using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities;

public class VanguardServiceHelper
{
    public const string VANGUARD_SERVICE_NAME = "vgk";

    public static bool IsVanguardRunning()
    {
        var VanguardServiceController = new ServiceController(VANGUARD_SERVICE_NAME);
        return VanguardServiceController.Status != ServiceControllerStatus.Stopped;
    }

    public static async Task<(bool success, Exception message)> TryStopVanguardAsync()
    {
        var VanguardServiceController = new ServiceController(VANGUARD_SERVICE_NAME);
        try
        {
            var vanguardStopTask = Task.Run(() => VanguardServiceController.Stop());
            await WaitForStatusAsync(VanguardServiceController, ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
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
}
