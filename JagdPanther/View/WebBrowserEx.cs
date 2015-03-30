using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JagdPanther.View
{

	public class WebBrowserEx : Grid
	{
		public WebBrowser Browser { get; set; }

		public WebBrowserEx()
		{
			Browser = new WebBrowser();
			HideScriptErrors(true);
			Children.Add(Browser);
		}
		public void HideScriptErrors(bool hide)
		{
			var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fiComWebBrowser == null) return;
			var objComWebBrowser = fiComWebBrowser.GetValue(Browser);
			if (objComWebBrowser == null)
			{
				Browser.Loaded += (o, s) => HideScriptErrors(hide);   //In case we are to early
				return;
			}
			objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
		}


		public string Html
		{
			get { return (string)GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Html.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof(string), typeof(WebBrowserEx), new PropertyMetadata(OnHtmlPropertyChanged));

		private static void OnHtmlPropertyChanged(
	DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var browser = obj as WebBrowserEx;

			if (browser == null)
			{
				return;
			}

			var html = args.NewValue as string;
			if (string.IsNullOrWhiteSpace(html))
			{
				return;
			}

			browser.Browser.NavigateToString(html);
		}
	}
}
