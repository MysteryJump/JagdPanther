using JagdPanther.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public class MultiSubredditViewModel : ReactiveObject
	{
		private ObservableCollection<Thread> threads;
		public MultiSubredditViewModel()
		{
			ThreadList = new ObservableCollection<Thread>();
			RefreshCommand = ReactiveCommand.CreateAsyncTask(RefreshExcute);
			SelectedCommand = ReactiveCommand.CreateAsyncTask(SelectedExcute);
			RemoveTabCommand = ReactiveCommand.CreateAsyncTask(RemoveTabExcute);
			RemoveAllTabCommand = ReactiveCommand.CreateAsyncTask(RemoveAllTabExcute);
		}

		public ObservableCollection<Thread> ThreadList
		{
			get { return threads; }
			set { threads = value; this.RaiseAndSetIfChanged(ref threads, value); }
		}

		private string name;

		private string path;

		public string Path
		{
			get { return path; }
			set { path = value; this.RaiseAndSetIfChanged(ref path, value); }
		}


		public string Name
		{
			get { return name; }
			set { name = value; this.RaiseAndSetIfChanged(ref name, value); }
		}

		public IReactiveCommand<Unit> RefreshCommand { get; set; }
		public IReactiveCommand<Unit> SelectedCommand { get; set; }
		public RedditData RedditInfo { get; internal set; }

		public async Task SelectedExcute(object sender)
		{
			if (ListViewSelectedItem == null)
				return;

			await ListViewSelectedItem.SubscribeComments();
			if (ListViewSelectedItem.SortedComments != null)
				MessageBus.Current.SendMessage(ListViewSelectedItem, "OpenNewThreadTab");
		}
		public async Task Initializer(string path)
		{


			var subs = RedditInfo.RedditAccess.GetMultireddit(path);
			Path = path;

			var l = await Task.Factory.StartNew(() =>
			{
				var lists = new List<Thread>();

				subs.Take(20)
					.ToList().ForEach(x =>
					{
						var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x, VoteCount = x.Upvotes - x.Downvotes, CommentCount = x.CommentCount };
						lists.Add(t);
					});
				return lists;
			});
			Name = path;

			l.ForEach(ThreadList.Add);
		}


		private Thread listViewSelectedItem;

		public Thread ListViewSelectedItem
		{
			get { return listViewSelectedItem; }
			set { listViewSelectedItem = value; this.RaiseAndSetIfChanged(ref listViewSelectedItem, value); }
		}

		public async Task RefreshExcute(object sender)
		{
			ThreadList.Clear();
			await Initializer(Path);
		}

		public IReactiveCommand<Unit> RemoveTabCommand { get; set; }
		public ReactiveCommand<Unit> RemoveAllTabCommand { get; private set; }

		public async Task RemoveTabExcute(object sender)
		{
			MessageBus.Current.SendMessage(this, "RemoveThreadListTab");
		}

		public async Task RemoveAllTabExcute(object sender)
		{
			MessageBus.Current.SendMessage("", "RemoveAllThreadListTab");

		}

		public async Task VoteExcute(object sender)
		{
			var r = sender.ToString();
			var p = ListViewSelectedItem;
			if (p == null)
				return;
			if (r == "Up")
			{
				p.PostThread.Upvote();
			}
			else
			{
				p.PostThread.Downvote();
			}
		}
	}
}
