using JagdPanther.Dialogs;
using JagdPanther.Model;
using ReactiveUI;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
    public class ThreadListViewModel : ReactiveObject
    {
        private ObservableCollection<Thread> threads;
        public ThreadListViewModel()
        {
            ThreadList = new ObservableCollection<Thread>();
            RefreshCommand = ReactiveCommand.CreateAsyncTask(RefreshExcute);
            SelectedCommand = ReactiveCommand.CreateAsyncTask(SelectedExcute);
            NewPostCommand = ReactiveCommand.CreateAsyncTask(NewPostExcute);
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
        public Subreddit OwnSubreddit { get; set; }
        public async Task Initializer(string path)
        {
            Path = path;
            var subs = OwnSubreddit = RedditInfo.RedditAccess.GetSubreddit(path);
            var lists = new List<Thread>();

            
            subs.Subscribe();
            subs.Posts.Take(50)
                .ToList().ForEach(x =>
                {
                    var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x,VoteCount = x.Upvotes - x.Downvotes, CommentCount = x.CommentCount };
                    lists.Add(t);
                });
            Name = subs.Name;
            
            lists.ForEach(ThreadList.Add);
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

        public IReactiveCommand<Unit> NewPostCommand { get; set; }
        public async Task NewPostExcute(object sender)
        {
            var sub = new SubmitWindow();
            sub.ShowDialog();
			Post p;
            if (sub.IsOk)
            {
                if (sub.IsLinkPost)
                {
                    p = OwnSubreddit.SubmitPost(sub.Title, sub.PostString);
					if (sub.IsNsfw)
						p.MarkNSFW();
					//if (sub.SelectedFlair.CssClass != null)
					//	p.SetFlair(sub.SelectedFlair.Text, sub.SelectedFlair.CssClass);
                }
                else
                {
                    p = OwnSubreddit.SubmitTextPost(sub.Title, sub.PostString);
					if (sub.IsNsfw)
						p.MarkNSFW();
					//if (sub.SelectedFlair.CssClass != null)
					//	p.SetFlair(sub.SelectedFlair.Text, sub.SelectedFlair.CssClass);

				}

			}
        }

		public IReactiveCommand<Unit> RemoveTabCommand { get;set; }
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
