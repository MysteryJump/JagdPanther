using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	public class SubredditSearcher
	{
		public static List<string> SearchSubreddit(string query)
		{
			var l = new List<string>();
			var wec = new WebClient();
			var data = wec.DownloadString("http://www.reddit.com/subreddits/search.json?q=" + query);
			var jk = JToken.Parse(data);
			var list = jk["data"]["children"].ToList();
			list.ForEach(x => l.Add(x["data"]["url"].ToString()));
			return l;
		}
	}
}
