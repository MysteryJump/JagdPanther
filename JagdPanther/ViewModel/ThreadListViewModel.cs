using JagdPanther.Dialogs;
using JagdPanther.Model;
using ReactiveUI;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public class ThreadListViewModel : ReactiveObject, IThreadListViewer, ITab
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
			SubscribeCommand = ReactiveCommand.CreateAsyncTask(SubscribeExcute);
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
        public RedditData RedditInfo { get; set; }

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
                            var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x, VoteCount = x.Upvotes - x.Downvotes, CommentCount = x.CommentCount + 1};
                            list.Add(t);
                        });
                    return list;
                });
                l.ForEach(ThreadList.Add);
                ThreadList.Remove(ListViewSelectedItem);
                ThreadList.Add(new Thread { Title = "次の20件を読み込む...", CommentCount = -1 });

                return;
            }
			var item = ListViewSelectedItem;
			await item.SubscribeComments();
			if (item.SortedComments != null)
				MessageBus.Current.SendMessage(item, "OpenNewThreadTab");
        }
        public Subreddit OwnSubreddit { get; set; }
		public async Task Initializer(string path)
        {


			Subreddit subs = null;
			Path = path;

            var l = await Task.Factory.StartNew(() =>
            {
				subs = OwnSubreddit = RedditInfo.RedditAccess.GetSubreddit(path);
				var lists = new List<Thread>();

                pageCount = 1;
                subs.Posts.Take(20)
                    .ToList().ForEach(x =>
                    {
						var t = new Thread()
						{
							Title = x.Title,
							CreatedTime = x.Created,
							PostThread = x,
							VoteCount = x.Upvotes - x.Downvotes,
							CommentCount = x.CommentCount +1,
							Id = x.Id,
							SubredditName = subs.Name,
							Flair = x.LinkFlairText
						};
                        lists.Add(t);
                    });
                return lists;
            });
            Name = subs.Title;
			var vp = path.Replace("/", "-").Remove(0,1).ToLower();
            using (var f = File.Open(Folders.PostListFolder + "\\" + vp + ".xml", FileMode.Create))
			{
				var li = (ThreadList)l;
				li.Subreddit = Name;
				li.SubredditPath = subs.Name;
                var dcs = new DataContractSerializer(typeof(ThreadList));
				dcs.WriteObject(f, li);
			}
			l.ForEach(ThreadList.Add);
            ThreadList.Add(new Thread { Title = "次の20件を読み込む...", CommentCount = -1 });
        }

		public async Task Initializer(string path,bool isOffline)
		{
			if (string.IsNullOrWhiteSpace(path))
				return;
			if (!isOffline)
				await Initializer(path);
			else
			{
				var vp = path.Replace("/", "-").Remove(0, 1).ToLower();
				var p = Folders.PostListFolder + "\\" + vp + ".xml";
				if (!File.Exists(p))
				{
					MessageBus.Current.SendMessage("Cannot open file: file dosen't exist.", "ErrorMessage");
					return;
				}
				using (var fs = File.Open(p, FileMode.Open))
				{
					var dcs = new DataContractSerializer(typeof(ThreadList));
					var da = dcs.ReadObject(fs) as ThreadList;
					var data = (List<Thread>)(da);
					Name = da.Subreddit;
					data.ForEach(ThreadList.Add);
					ThreadList.ToList().ForEach(x => x.SubredditName = da.SubredditPath);
				}
			}
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
					var pos = new PostingBeforeProcessor(sub.PostString);
					pos.ReplaceEndOfLine();
					p = OwnSubreddit.SubmitPost(sub.Title, pos.ProcessedText);
					if (sub.IsNsfw)
						p.MarkNSFW();
					if (string.IsNullOrWhiteSpace(sub.Flair))
						p.SetFlair(sub.Flair, "");
				}
                else
                {
                    p = OwnSubreddit.SubmitTextPost(sub.Title, sub.PostString);
					if (sub.IsNsfw)
						p.MarkNSFW();
					if (string.IsNullOrWhiteSpace(sub.Flair))
						p.SetFlair(sub.Flair, "");
				}

			}
        }

		public IReactiveCommand<Unit> RemoveTabCommand { get;set; }
		public IReactiveCommand<Unit> RemoveAllTabCommand { get; set; }

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

		public IReactiveCommand<Unit> SubscribeCommand { get; set; }

		public async Task SubscribeExcute(object sender)
		{
			OwnSubreddit.Subscribe();
		}

		public bool IsSubscribed { get { return false; } }

		public string Title
		{
			get
			{
				return Name;
			}

			set
			{
				Name = value;
			}
		}
	}
}
