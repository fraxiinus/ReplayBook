using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rofl.UI.Main.Utilities
{
    public static class ResourceTools
    {
        internal static T GetObjectFromResource<T>(string resourceName)
        {
            return (T)Application.Current.TryFindResource(resourceName);
        }

        internal static ImageSource GetImageSourceFromResource(string resourceName)
        {
            return Application.Current.TryFindResource(resourceName) as ImageSource;
        }

        internal static ImageSource GetImageSourceFromPath(string path)
        {
            return BitmapFrame.Create(new Uri(path), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        internal static SolidColorBrush GetColorFromResource(string key)
        {
            return Application.Current.TryFindResource(key) as SolidColorBrush;
        }
    }
}
