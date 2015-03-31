using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;
using RedditSharp;
using System.Net;
using ReactiveUI;

namespace JagdPanther.Model
{
    public class RedditControl
    {
		[Obsolete("Use Login(string refreshToken) instead")]
        public static RedditData Login()
        {
			var log = new OAuthLoginInfo();
			log.LoadData();
			return Login(log.RefreshToken);
        }

		public static RedditData Login(string refreshToken)
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
	}

    public class RedditData
    {
        public Reddit RedditAccess { get; set; }
        public AuthenticatedUser RedditUser { get; set; }
		public RedditData()
		{
			MessageBus.Current.Listen<RedditData>("ChangeAccount-3")
				.Subscribe(x =>
				{
					if (x == null)
						return;
					RedditAccess = x.RedditAccess;
					RedditUser = x.RedditUser;
				});
		}
    }
}
