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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Set = JagdPanther.Properties.Settings;

namespace JagdPanther.View
{
	/// <summary>
	/// EnviromentSetting.xaml の相互作用ロジック
	/// </summary>
	public partial class EnviromentSetting : UserControl, ISettingControl
	{
		public EnviromentSetting()
		{
			InitializeComponent();
		}

		public void Save()
		{
			Set.Default.IsWebView = iswebview.IsChecked == true ? true : false;
			Set.Default.IsTreeView = istreeview.IsChecked == true ? true : false;
			Set.Default.IsShowBakaStamp = isshowbaka.IsChecked == true ? true : false;
			Set.Default.IsSaveThreadListView = issavethreadlisttabs.IsChecked == true ? true : false;
			Set.Default.IsSaveThreadView = issavethreadtabs.IsChecked == true ? true : false;
		}

		public void Load()
		{
			iswebview.IsChecked = Set.Default.IsWebView;
			istreeview.IsChecked = Set.Default.IsTreeView;
			isshowbaka.IsChecked = Set.Default.IsShowBakaStamp;
			issavethreadtabs.IsChecked = Set.Default.IsSaveThreadView;
			issavethreadlisttabs.IsChecked = Set.Default.IsSaveThreadListView;
		}

		private void issavethreadtabs_Checked(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
			{
				istreeview.IsEnabled = false;
				istreeview.IsChecked = false;
			}
			else
			{
				istreeview.IsEnabled = true;
			}
		}

		private void istreeview_Checked(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
			{
				issavethreadtabs.IsEnabled = false;
				issavethreadtabs.IsChecked = false;

				isshowbaka.IsEnabled = false;
				isshowbaka.IsChecked = false;
			}
			else
			{
				isshowbaka.IsEnabled = true;
				issavethreadtabs.IsEnabled = true;
			}
		}

		private void iswebview_Checked(object sender, RoutedEventArgs e)
		{
			if ((sender as CheckBox).IsChecked == true)
			{
				istreeview.IsChecked = false;
				istreeview.IsEnabled = false;
			}
			else
			{
				istreeview.IsEnabled = true;
			}
		}
	}
}
