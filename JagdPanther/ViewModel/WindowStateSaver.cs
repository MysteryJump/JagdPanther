using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using JagdPanther.Model;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace JagdPanther.ViewModel
{
	public class WindowStateSaver
	{
		private MainViewModel mvm;
		public WindowStateSaver(MainViewModel mv)
		{
			mvm = mv;
		}

		public void SaveThreadListTabs()
		{
			var jo = new JArray();

			mvm.ThreadListTabs.ThreadListChildrens
				.ToList()
				.ForEach(x =>
				{
					var ja = new JObject();
					ja.Add("name", x.Name);
					ja.Add("path", x.Path);
					ja.Add("type", x.GetType().ToString());
					jo.Add(ja);
				});
			File.WriteAllText(Folders.ThreadListTabsJson, jo.ToString());
		}

		public void LoadThreadListTabs()
		{
			var data = File.Exists(Folders.ThreadListTabsJson) ?
				File.ReadAllText(Folders.ThreadListTabsJson) : string.Empty;
			if (string.IsNullOrEmpty(data))
				return;
			var isFirst = true;
			JArray.Parse(data)
				.ToList()
				.ForEach(async x =>
				{
					IThreadListViewer itlv = null;
					if (x["type"].ToString().Contains("MultiSubreddit"))
						itlv = new MultiSubredditViewModel();
					else
						itlv = new ThreadListViewModel();

					await itlv.Initializer(x["path"].ToString(), true);
					
					mvm.ThreadListTabs.ThreadListChildrens.Add(itlv);
					if (isFirst)
					{
						MessageBus.Current.SendMessage(itlv, "ThreadListSelectTab");
						isFirst = false;
					}
				});
			
		}

		public void SaveThreadTabs()
		{
			var ja = new JArray();

			mvm.ThreadTabs.ThreadTabsChildren
				.ToList()
				.ForEach(x =>
				{
					var jo = new JObject();
					jo.Add("id", x.Id);
					jo.Add("subreddit", x.SubredditName);
					jo.Add("title", x.Title);
					ja.Add(jo);
				});
			File.WriteAllText(Folders.ThreadTabsJson, ja.ToString());
		}

		public void LoadThreadTabs()
		{
			var data = File.Exists(Folders.ThreadTabsJson) ?
				File.ReadAllText(Folders.ThreadTabsJson) : string.Empty;
			if (string.IsNullOrEmpty(data))
				return;
			var isFirst = true;
			JArray.Parse(data)
				.ToList()
				.ForEach(async x =>
				{
					var th = new Thread()
					{
						Id = x["id"].ToString(),
						SubredditName = x["subreddit"].ToString(),
						Title = x["title"].ToString()
					};

					MainViewModel.IsOffline = true;
					await th.SubscribeComments();
					mvm.ThreadTabs.ThreadTabsChildren.Add(th);
					MainViewModel.IsOffline = false;

					if (isFirst)
					{
						MessageBus.Current.SendMessage(th, "ThreadSelectTab");
						isFirst = false;
					}
				});
		}
	}
}
