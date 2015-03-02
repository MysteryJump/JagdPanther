using JagdPanther.Dialogs;
using JagdPanther.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            RedditInfo = RedditControl.Login();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            ThreadListTabs = new ThreadListTabsViewModel();
            ThreadListTabs.ThreadListChildrens.CollectionChanged += ThreadListChildrens_CollectionChanged;
            ThreadListTabs.ThreadListChildrens.Add(new ThreadListViewModel());
            ThreadTabs = new ThreadTabsViewModel();
            MessageBus.Current.Listen<Thread>("OpenNewThreadTab").Subscribe(x =>
                {
                    ThreadTabs.ThreadTabsChildren.Add(x);
                });
            ExitCommand = ReactiveCommand.CreateAsyncTask(ExitExcute);
            OpenLicenseWindowCommand = ReactiveCommand.CreateAsyncTask(OpenLicenseWindowExcute);
            OpenVersionWindowCommand = ReactiveCommand.CreateAsyncTask(OpenVersionWindowExcute);
            Title = ReadonlyVars.ProgramName+" "+ReadonlyVars.ProgramVer;
        }

        private void ThreadListChildrens_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ((ThreadListViewModel)(e.NewItems[0])).RedditInfo = RedditInfo;
            ((ThreadListViewModel)(e.NewItems[0])).Initializer("/r/newsokur");
        }

        public RedditData RedditInfo { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        public IReactiveCommand<Unit> ExitCommand { get; set; }        
        public async Task ExitExcute(object o)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public IReactiveCommand<Unit> OpenLicenseWindowCommand { get; set; }

        public async Task OpenLicenseWindowExcute(object sender)
        {
            var w = new LicenseInfo();
            w.ShowDialog();
        }
        public IReactiveCommand<Unit> OpenVersionWindowCommand { get; set; }

        public async Task OpenVersionWindowExcute(object sender)
        {
            var w = new VersionWindow();
            w.ShowDialog();
        }

        //public ThreadListViewModel ThreadList { get; set; }

        public ThreadListTabsViewModel ThreadListTabs { get; set; }

        public ThreadTabsViewModel ThreadTabs { get; set; }
    }
}
