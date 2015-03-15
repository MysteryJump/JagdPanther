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
            if (Properties.Settings.Default.IsLoggedin)
            {
                try
				{
					var log = new OAuthLoginInfo();
					log.LoadData();
					var ac = log.GetNewAccessToken();
					var red = new Reddit(ac);
					var user = red.User;
                    return new RedditData { RedditAccess = red, RedditUser = user };
                }
                catch (WebException e)
                {
                    System.Windows.MessageBox.Show("ログインできませんでした");
                    return null;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

		internal static RedditData Login(Account a)
		{
			try
			{
				var red = new Reddit();
				var user = red.LogIn(a.UserName, a.Password);
				return new RedditData { RedditAccess = red, RedditUser = user };
			}
			catch (WebException e)
			{
				System.Windows.MessageBox.Show("ログインできませんでした");
				return null;
			}

		}
	}

    public class RedditData
    {
        public Reddit RedditAccess { get; set; }
        public AuthenticatedUser RedditUser { get; set; }
    }
}
