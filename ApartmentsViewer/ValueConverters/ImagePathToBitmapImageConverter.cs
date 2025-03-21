using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ApartmentsViewer.ValueConverters
{
	public class ImagePathToBitmapImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string imagePath)
			{
				if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
				{
					return null;
				}

				BitmapImage bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
				bitmap.CacheOption = BitmapCacheOption.OnLoad; 
				bitmap.EndInit();
				return bitmap;
			}

			return null; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// ConvertBack is typically not needed for ImageSource conversions from path,
			// as binding is usually one-way (path to image display)
			throw new NotSupportedException("ConvertBack not supported for ImagePath to BitmapImage conversion.");
		}
	}
}
