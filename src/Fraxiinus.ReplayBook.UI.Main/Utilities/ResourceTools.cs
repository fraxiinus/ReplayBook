using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
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

        /// <summary>
        /// Given file path, loads image into BitmapFrame (ImageSource)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static BitmapFrame GetImageSourceFromPath(string path)
        {
            return BitmapFrame.Create(new Uri(path), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        }

        internal static SolidColorBrush GetColorFromResource(string key)
        {
            return System.Windows.Application.Current.TryFindResource(key) as SolidColorBrush;
        }
    }
}
