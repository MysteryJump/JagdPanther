using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JagdPanther
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
			if (!File.Exists(Folders.LoginInfoXml))
			{
				var s = new Dialogs.OAuthLoginWindow();
				s.ShowDialog();
				new OAuthLoginInfo() { RefreshToken = s.RefreshToken }.SaveData();
				JagdPanther.Properties.Settings.Default["IsLoggedin"] = true;
			}
			else
			{
				JagdPanther.Properties.Settings.Default["IsLoggedin"] = true;
			}
		}
    }
}
