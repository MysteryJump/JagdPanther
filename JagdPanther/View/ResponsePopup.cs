using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagdPanther.Model;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace JagdPanther.View
{
	public class ResponsePopup
	{
		internal static void ShowParentPopup(TextBlock comment)
		{
			Point swp = new Point(System.Windows.Forms.Cursor.Position.X,System.Windows.Forms.Cursor.Position.Y);
			var w = new ShowPopupWindow();
			w.Background = new SolidColorBrush(Properties.Settings.Default.ThreadViewBackgroundColor);
			w.SetItem((comment.DataContext as ViewComment).Parent);	
			//swp = Application.Current.MainWindow.PointFromScreen(swp);

			w.Left = swp.X;
			w.Top = swp.Y;
			w.Show();
		}

		internal static void ShowIdPopup(TextBlock comment)
		{
			Point swp = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
			var w = new ShowPopupWindow();
			var v = (comment.DataContext as ViewComment);
            w.Background = new SolidColorBrush(Properties.Settings.Default.ThreadViewBackgroundColor);
			var z = v.ParentPost.Comments.Where(x => x.Author == v.Author).ToList();
			var lis = new List<ViewComment>();
			z.ForEach(x => lis.Add((ViewComment)x));
            w.SetItems(lis);
			//swp = Application.Current.MainWindow.PointFromScreen(swp);

			w.Left = swp.X;
			w.Top = swp.Y;
			w.Show();
		}
	}
}
