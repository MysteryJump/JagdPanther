using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	public static class ProgramInitializer
	{
		public static void CheckFolders()
		{

			var lists = new List<string>()
			{
				Folders.CommentListFolder,
				Folders.PostListFolder,
				Folders.StyleFolder,
				Folders.ThreadStyleFolder
			};
			CheckTargetFolder(lists);
		}

		private static void CheckTargetFolder(string path)
		{
			if (!File.Exists(path))
				Directory.CreateDirectory(path);
		}

		private static void CheckTargetFolder(List<string> paths)
		{
			paths.ForEach(CheckTargetFolder);
		}

		public static void LoginCheck()
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			if (!File.Exists(Folders.LoginInfoXml))
			{
				var s = new Dialogs.OAuthLoginWindow();
				s.ShowDialog();
				var ac = new AccountList()
				{
					Accounts = new List<Account>()
				};
				var a = new Account() { RefreshToken = s.RefreshToken };
				a.GetUserName();
				ac.Accounts.Add(a);
				ac.LoggedAccount = a;
				ac.Save();
			}
		}

		public static void DownloadBakaStamp()
		{
			var wec = new WebClient();
			wec.Encoding = Encoding.UTF8;
			var baka = wec.DownloadString("http://b.thumbs.redditmedia.com/E6_NeFiFS0CEzBgII10A0JiVJA_Qx5H7lZAxjzH4FdY.css");
			//wec.DownloadFile("http://b.thumbs.redditmedia.com/njJmvGN935v6aDIPsJjhQo1prIBUjJ6pEgZiMIBw73M.png", Folders.BakaStampImg);

			var v = baka.Replace("//", "http://");
			File.WriteAllText(Folders.BakaStamp, v);
		}
	}
}
