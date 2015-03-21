using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;
using RedditSharp;
using System.Net;

namespace JagdPanther.Model
{
    public class RedditControl
    {
        public static RedditData Login()
        {
			var log = new OAuthLoginInfo();
			log.LoadData();
			return Login(log.RefreshToken);
        }

		public static RedditData Login(string refreshToken)
		{
			if (Properties.Settings.Default.IsLoggedin)
			{
				while (true)
				{
					try
					{
						var ac = OAuthLoginInfo.GetNewAccessToken(refreshToken);
						var red = new Reddit(ac);
						var user = red.User;
						return new RedditData { RedditAccess = red, RedditUser = user };
					}
					catch (WebException e)
					{
						var s = System.Windows.MessageBox.Show("ログインできませんでした\r\n再試行しますか？", "ログイン", System.Windows.MessageBoxButton.YesNo);
						if (s == System.Windows.MessageBoxResult.No)
						{
							return null;
						}
					}
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
	}

    public class RedditData
    {
        public Reddit RedditAccess { get; set; }
        public AuthenticatedUser RedditUser { get; set; }
    }
}
