using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther
{
	public static class ExtensionMethods
	{
		public static DateTime LastTime { get;set; }
		private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long ToUnixTimeSeconds(this DateTimeOffset dateTime)
		{
			double nowTicks = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
			return (long)nowTicks;
		}
		// see also http://kiwigis.blogspot.jp/2010/12/how-to-add-scrollintoview-to.html
		public static void ScrollIntoView(
			this ItemsControl control,
			object item)
		{
			FrameworkElement framework =
				control.ItemContainerGenerator.ContainerFromItem(item)
				as FrameworkElement;
			if (framework == null) { return; }
			framework.BringIntoView();
		}
		public static void ScrollIntoView(this ItemsControl control)
		{
			int count = control.Items.Count;
			if (count == 0) { return; }
			object item = control.Items[count - 1];
			control.ScrollIntoView(item);
		}

	}
}
