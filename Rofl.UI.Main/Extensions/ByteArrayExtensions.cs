using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Rofl.UI.Main.Extensions
{
    public static class ByteArrayExtensions
    {
        public static BitmapImage ToBitmapImage(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;

            var image = new BitmapImage();

            using (var mem = new MemoryStream(bytes))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            image.Freeze();
            return image;
        }
    }
}
