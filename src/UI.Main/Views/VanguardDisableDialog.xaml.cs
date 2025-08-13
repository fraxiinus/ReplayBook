using Fraxiinus.ReplayBook.UI.Main.Utilities;
using ModernWpf.Controls;
using System;

namespace Fraxiinus.ReplayBook.UI.Main.Views;

/// <summary>
/// Interaction logic for VanguardDisableDialog.xaml
/// </summary>
public partial class VanguardDisableDialog : ContentDialog
{
    public Exception Exception { get; private set; }
    public bool Success { get; private set; }

    public VanguardDisableDialog()
    {
        InitializeComponent();
    }

    private async void DisableVanguardDialog_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (DisableVanguardDialog.IsVisible)
        {
            var (success, exception) = await VanguardServiceHelper.StartPrivilegedProcess();
            if (!success)
            {
                Exception = exception;
            }
            Success = success;
            Hide();
        }
    }
}
