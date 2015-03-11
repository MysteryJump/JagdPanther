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

namespace JagdPanther.View
{
	[ValueConversion(typeof(ViewComment), typeof(StackPanel))]
	public class ViewCommentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var vc = (ViewComment)value;
            var sp = new StackPanel();
            sp.Margin = new Thickness(0, 5, 0, 5);

            var tx1 = new TextBlock();
            tx1.Text = vc.CommentNumber.ToString();

            var tx2 = new TextBlock();
            tx2.Text = ":";

            var tx3 = new TextBlock();
            tx3.Text = vc.Author;

            var tx4 = new TextBlock();
            tx4.Text = "[";

            var tx5 = new TextBlock();
            tx5.Text = vc.FlairText;

            var tx6 = new TextBlock();
            tx6.Text = "]  Vote:";

            var tx7 = new TextBlock();
            tx7.Text = vc.Votes.ToString();

            var tx8 = GenerateVoteCommandsText(vc);

            return new StackPanel();
		}

        private object GenerateVoteCommandsText(ViewComment vc)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
