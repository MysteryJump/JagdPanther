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
		private static FileStream locker;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
			bool a = false;
			if (File.Exists(ReadonlyVars.CurrentFolder + "\\lock"))
				a = true;
			try
			{
				locker = File.Create(ReadonlyVars.CurrentFolder + "\\lock");
			}
			catch
			{
				a = true;
			}

			if (a)
			{
				MessageBox.Show("多重起動してる悪い子はだ～れぇだ？");
				Current.Shutdown(-1);
				System.Threading.Thread.Sleep(5000);
				return;
			}

			locker.Lock(0, 6);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (e.ApplicationExitCode == -1)
				return;
			locker.Unlock(0, 6);
			locker.Close();
			File.Delete(ReadonlyVars.CurrentFolder + "\\lock");
		}
	}
}
