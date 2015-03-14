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
				var s = new Dialogs.LoginAndGenerateNewUserWindow();
				s.ShowDialog();
				new LoginInfo() { Name = s.Data.UserName, Password = s.Data.Password }.SaveData();
				Application.Current.Shutdown();
				System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
				JagdPanther.Properties.Settings.Default["IsLoggedin"] = true;
			}
			else
			{
				JagdPanther.Properties.Settings.Default["IsLoggedin"] = true;
			}
		}
    }
}
