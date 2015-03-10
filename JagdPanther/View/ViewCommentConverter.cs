using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;

namespace JagdPanther.View
{
	[ValueConversion(typeof(ViewComment), typeof(TextBlock))]
	public class ViewCommentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var vc = (ViewComment)value;
			return new TextBlock();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
