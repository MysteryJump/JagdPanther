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
        private int pageCount;
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
            if (ListViewSelectedItem.CommentCount == -1)
            {

                var l = await Task.Factory.StartNew(() =>
                {
                    var list = new List<Thread>();

                    OwnSubreddit.Posts.Skip(pageCount * 20)
                        .Take(++pageCount * 20)
                        .ToList()
                        .ForEach(x =>
                        {
                            var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x, VoteCount = x.Upvotes - x.Downvotes, CommentCount = x.CommentCount };
                            list.Add(t);
                        });
                    return list;
                });
                l.ForEach(ThreadList.Add);
                ThreadList.Remove(ListViewSelectedItem);
                ThreadList.Add(new Thread { Title = "次の20件を読み込む...", CommentCount = -1 });

                return;
            }
            await ListViewSelectedItem.SubscribeComments();
            if (ListViewSelectedItem.SortedComments != null)
                MessageBus.Current.SendMessage(ListViewSelectedItem, "OpenNewThreadTab");
        }
        public Subreddit OwnSubreddit { get; set; }
        public async Task Initializer(string path)
        {
            var subs = OwnSubreddit = RedditInfo.RedditAccess.GetSubreddit(path);
            Path = path;

            var l = await Task.Factory.StartNew(() =>
            {
                var lists = new List<Thread>();
                subs.Subscribe();

                pageCount = 1;
                subs.Posts.Take(20)
                    .ToList().ForEach(x =>
                    {
                        var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x, VoteCount = x.Upvotes - x.Downvotes, CommentCount = x.CommentCount };
                        lists.Add(t);
                    });
                return lists;
            });
            Name = subs.Name;
            
            l.ForEach(ThreadList.Add);
            ThreadList.Add(new Thread { Title = "次の20件を読み込む...", CommentCount = -1 });
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
