using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;
using RedditSharp;

namespace JagdPanther.Model
{
    public class RedditControl
    {
        public static RedditData Login()
        {
            if (Properties.Settings.Default.IsLoggedin)
            {
                var red = new Reddit();
                var log = new LoginInfo();
                log.LoadData();
                var user = red.LogIn(log.Name, log.Password);
                return new RedditData { RedditAccess = red, RedditUser = user };
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
