using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace SmartCardExplorer.Windows.WPF
{
	[ValueConversion(typeof(Bitmap), typeof(ImageSource))]
	public class BitmapToImageSourceConverter : IValueConverter
	{
		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			if (!(value is Bitmap))
				return null;

			return CreateBitmapSourceFromGdiBitmap(value as Bitmap);
		}

		public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");

			var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			var bitmapData = bitmap.LockBits(
			 rect,
			 ImageLockMode.ReadWrite,
			 System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			try
			{
				var size = (rect.Width * rect.Height) * 4;

				return BitmapSource.Create(
				 bitmap.Width,
				 bitmap.Height,
				 bitmap.HorizontalResolution,
				 bitmap.VerticalResolution,
				 PixelFormats.Bgra32,
				 null,
				 bitmapData.Scan0,
				 size,
				 bitmapData.Stride);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}

		public static BitmapSource CreateBitmapSourceFromGdiBitmapGDI(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");

			using (Stream str = new MemoryStream())
			{
				bitmap.Save(str, System.Drawing.Imaging.ImageFormat.Bmp);
				str.Seek(0, SeekOrigin.Begin);
				BitmapDecoder bdc = new BmpBitmapDecoder(str, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
				return bdc.Frames[0];
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
