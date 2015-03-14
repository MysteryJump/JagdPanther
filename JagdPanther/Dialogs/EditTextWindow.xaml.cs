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

namespace JagdPanther.Dialogs
{
	/// <summary>
	/// EditTextWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class EditTextWindow : Window
	{
		public EditTextWindow()
		{
			InitializeComponent();
		}

		public string TextBoxText
		{
			get { return x.Text; }
			set { x.Text = value; }
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
