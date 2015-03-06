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
                    var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x,VoteCount = x.Upvotes - x.Downvotes };
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
            var count = ThreadList.Count;
            Enumerable.Range(0, count)
                .Select(x => x = 0)
                .ToList()
                .ForEach((x) =>
                {
                    ThreadList.RemoveAt(x);
                });
            Initializer(Path);
        }

        public IReactiveCommand<Unit> NewPostCommand { get; set; }
        public async Task NewPostExcute(object sender)
        {
            var sub = new SubmitWindow();
            sub.ShowDialog();
            if (sub.IsOk)
            {
                if (sub.IsLinkPost)
                {
                    OwnSubreddit.SubmitPost(sub.Title, sub.PostString);
                }
                else
                {
                    OwnSubreddit.SubmitTextPost(sub.Title, sub.PostString);
                }
            }
        }
    }
}
