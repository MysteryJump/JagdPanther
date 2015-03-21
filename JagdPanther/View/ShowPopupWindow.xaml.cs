using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JagdPanther.View
{
	/// <summary>
	/// ShowPopupWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ShowPopupWindow : Window
	{
		public ShowPopupWindow()
		{
			InitializeComponent();
			Background = new SolidColorBrush(Properties.Settings.Default.ThreadPopupViewBackgroundColor);
		}

		private void Window_MouseLeave(object sender, MouseEventArgs e)
		{
			this.Close();
		}

		public void SetItem(ViewComment vc)
		{
			ddds.Items.Add(vc);
		}

		public void SetItems(List<ViewComment> vc)
		{
			vc.ForEach(x => ddds.Items.Add(x));
		}
	}
}
