using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace JagdPanther.View
{
	[ValueConversion(typeof(Color),typeof(SolidColorBrush))]
	public class ColorToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var col = (Color)value;
			return new SolidColorBrush(col);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var brush = value as SolidColorBrush;
			return brush.Color;
		}
	}
}
