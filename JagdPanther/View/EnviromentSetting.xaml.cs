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
		}

		public void Load()
		{
		}

	}
}
