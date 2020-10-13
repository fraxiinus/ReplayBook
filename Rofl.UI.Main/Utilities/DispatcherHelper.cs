using System;
using System.Windows;
using System.Windows.Threading;

namespace Rofl.UI.Main.Utilities
{
    /// <summary>
    /// DispatcherHelper from ModernWPF SamplesCommon by Yimeng Wu
    /// https://github.com/Kinnara/ModernWpf
    /// </summary>
    public static class DispatcherHelper
    {
        public static void RunOnMainThread(Action action)
        {
            RunOnUIThread(Application.Current, action);
        }

        public static void RunOnUIThread(this DispatcherObject d, Action action)
        {
            if (d == null) throw new ArgumentNullException(nameof(d));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var dispatcher = d.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }
}
