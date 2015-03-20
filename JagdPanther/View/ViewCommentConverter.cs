using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Text.RegularExpressions;

namespace JagdPanther.View
{
	[ValueConversion(typeof(string), typeof(TextBlock))]
	public class ViewCommentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();

			var v = value.ToString();
			var lines = v.Split('\n');
			foreach (var item in lines)
			{

			}
		}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
