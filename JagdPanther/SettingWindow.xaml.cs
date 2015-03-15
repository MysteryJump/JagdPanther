using ReactiveUI;
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

namespace JagdPanther
{
	/// <summary>
	/// SettingWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class SettingWindow : Window
	{
		public SettingWindow()
		{
			InitializeComponent();
			MessageBus.Current.Listen<string>("CloseSettingWindow").Subscribe(_ => this.Close());
			RegisterTags();
		}

		private void RegisterTags()
		{
			enviroment.Tag = new View.EnviromentSetting();
		}

		public void SelectedItem(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace((sender as TreeViewItem).Name))
			{
				var data = ((sender as TreeViewItem).Tag as UIElement);
				d.Content = data;
			}
			
		}
	}
}
