using JagdPanther.View;
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
using System.ComponentModel;

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
			MessageBus.Current.Listen<string>("CloseSettingWindow").Subscribe(_ =>
			{
				Close();
			});
			RegisterTags();
		}

		private void RegisterTags()
		{
			enviroment.Tag = new EnviromentSetting();
		}

		public void SelectedItem(object sender, RoutedEventArgs e)
		{
			if (beforeData != null)
				beforeData.Save();
			var tree = (sender as TreeViewItem).Tag;
			if (tree == null)
				return;
            if (!string.IsNullOrWhiteSpace((sender as TreeViewItem).Name))
			{
				var data = (tree as UserControl);
				d.Content = data;
				var dr = tree as ISettingControl;
				dr.Load();
			}
			beforeData = tree as ISettingControl;
		}

		private ISettingControl beforeData;

		protected override void OnClosing(CancelEventArgs e)
		{
			if (DialogResult == false)
				Properties.Settings.Default.Reload();
			else
				Properties.Settings.Default.Save();
			base.OnClosing(e);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			beforeData.Save();
			Close();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
