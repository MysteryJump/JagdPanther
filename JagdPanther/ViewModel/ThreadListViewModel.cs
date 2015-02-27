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
            SelectedCommand = ReactiveCommand.CreateAsyncTask(SelectedExcute);
        }

        public ObservableCollection<Thread> ThreadList
        {
            get { return threads; }
            set { threads = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; this.RaiseAndSetIfChanged(ref name, value); }
        }


        public IReactiveCommand<Unit> SelectedCommand { get; set; }
        public RedditData RedditInfo { get; internal set; }

        public async Task SelectedExcute(object sender)
        {
            MessageBus.Current.SendMessage(ListViewSelectedItem, "OpenNewThreadTab");
        }

        public void Initializer(string path)
        {
            var subs = RedditInfo.RedditAccess.GetSubreddit(path);
            subs.Subscribe();
            subs.Posts.Take(100)
                .ToList().ForEach(x =>
                {
                    var t = new Thread() { Title = x.Title, CreatedTime = x.Created, PostThread = x};
                    ThreadList.Add(t);
                });
            Name = subs.Name;
        }

        private Thread listViewSelectedItem;

        public Thread ListViewSelectedItem
        {
            get { return listViewSelectedItem; }
            set { listViewSelectedItem = value; this.RaiseAndSetIfChanged(ref listViewSelectedItem, value); }
        }

    }
}
