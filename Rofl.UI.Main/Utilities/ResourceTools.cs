using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rofl.UI.Main.Utilities
{
    public static class ResourceTools
    {
        internal static T GetObjectFromResource<T>(string resourceName)
        {
            return (T)System.Windows.Application.Current.TryFindResource(resourceName);
        }

        internal static ImageSource GetImageSourceFromResource(string resourceName)
        {
            return System.Windows.Application.Current.TryFindResource(resourceName) as ImageSource;
        }

        internal static ImageSource GetImageSourceFromPath(string path)
        {
            return BitmapFrame.Create(new Uri(path), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        internal static SolidColorBrush GetColorFromResource(string key)
        {
            return System.Windows.Application.Current.TryFindResource(key) as SolidColorBrush;
        }
    }
}
