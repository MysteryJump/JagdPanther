using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
		private static Mutex mux;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
			ProgramInitializer.CheckFolders();

			mux = new Mutex(false, "JagdPanther-Browser");
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
				bool isExit = true;
				if (!mux.WaitOne(0, false))
				{
					MessageBox.Show("多重起動は禁止されています");
				}
				else
				{
					var x = MessageBox.Show("多重起動してないのにlockファイルが残っています\r\n続行しますか？", "お知らせ", MessageBoxButton.YesNo);
					if (x == MessageBoxResult.Yes)
					{
						isExit = false;
					}

				}
				if (isExit)
					Environment.Exit(-1);
			}

			locker.Lock(0, 6);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (e.ApplicationExitCode == -1)
				return;
			locker.Unlock(0, 6);
			locker.Close();
			try
			{
				mux.ReleaseMutex();
				mux.Close();
			}catch { }
			File.Delete(ReadonlyVars.CurrentFolder + "\\lock");
		}
	}
}
