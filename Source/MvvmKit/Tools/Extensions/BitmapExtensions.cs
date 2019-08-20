using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MvvmKit
{
    public static class BitmapExtensions
    {
        public static BitmapSource ToBitmapSource(this byte[] bitmap, bool isFrozen = false)
        {
            BitmapImage res = null;

            if (bitmap != null)
            {
                res = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(bitmap))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    res.BeginInit();
                    res.StreamSource = stream;
                    res.CacheOption = BitmapCacheOption.OnLoad;
                    res.EndInit();
                    if (isFrozen) res.Freeze();
                }
            }
            return res;
        }

        public static ImageSource ToImageSource(this byte[] bitmap, bool isFrozen = false)
        {
            var bs = bitmap.ToBitmapSource(isFrozen);
            return bs;
        }

        public static BitmapSource ToBitmapSource(this Bitmap bitmap, bool isFrozen = false)
        {
            BitmapSource res = null;

            if (bitmap != null)
            {
                res = Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                if (isFrozen) res.Freeze();
            }

            return res;
        }

        public static ImageSource ToImageSource(this Bitmap bitmap, bool isFrozen = false)
        {
            var bs = bitmap.ToBitmapSource(isFrozen);
            return bs;
        }

        public static byte[] ToByteArray(this BitmapSource bitmapSource)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            byte[] bit = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                bit = stream.ToArray();
                stream.Close();
            }

            return bit;
        }

        public static byte[] ToByteArray(this ImageSource imageSource)
        {
            var bs = imageSource as BitmapSource;
            return bs.ToByteArray();
        }
    }
}
