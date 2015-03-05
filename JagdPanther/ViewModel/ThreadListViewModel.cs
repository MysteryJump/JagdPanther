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
    public class ThreadListViewModel : ReactiveObject
    {
        private ObservableCollection<Thread> threads;
        public ThreadListViewModel()
        {
            ThreadList = new ObservableCollection<Thread>();
            RefreshCommand = ReactiveCommand.CreateAsyncTask(RefreshExcute);
            SelectedCommand = ReactiveCommand.CreateAsyncTask(SelectedExcute);
        }

        public ObservableCollection<Thread> ThreadList
        {
            get { return threads; }
            set { threads = value; }
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
            MessageBus.Current.SendMessage(ListViewSelectedItem, "OpenNewThreadTab");
        }

        public async Task Initializer(string path)
        {
            Path = path;
            var subs = RedditInfo.RedditAccess.GetSubreddit(path);
            var lists = new List<Thread>();

            subs.Subscribe();

            subs.Posts.Take(20)
                .ToList().ForEach(x =>
                {

                    var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x };
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

    }
}
