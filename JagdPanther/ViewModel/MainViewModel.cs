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
            if (RedditInfo == null)
                App.Current.Shutdown();
            else
                RegisterCommands();
        }

        private void RegisterCommands()
        {
            ThreadListTabs = new ThreadListTabsViewModel();
			SubredditList = new SubredditListViewModel();
			MessageBus.Current.Listen<Board>("OpenNewSubreddit").Subscribe(async (x) =>
			{
				var v = new ThreadListViewModel();
                v.RedditInfo = RedditInfo;
				await v.Initializer(x.Path);
                ThreadListTabs.ThreadListChildrens.Add(v);
			});

            ThreadTabs = new ThreadTabsViewModel();
            MessageBus.Current.Listen<Thread>("OpenNewThreadTab").Subscribe(x =>
                {
                    ThreadTabs.ThreadTabsChildren.Add(x);
                });
            MessageBus.Current.Listen<string>("ErrorMessage").Subscribe(x =>
                {
                    ErrorMessage = x;
                });
            ExitCommand = ReactiveCommand.CreateAsyncTask(ExitExcute);
			AddNewSubredditCommand = ReactiveCommand.CreateAsyncTask(AddNewSubredditExcute);
            OpenLicenseWindowCommand = ReactiveCommand.CreateAsyncTask(OpenLicenseWindowExcute);
            OpenVersionWindowCommand = ReactiveCommand.CreateAsyncTask(OpenVersionWindowExcute);
            Title = ReadonlyVars.ProgramName+" "+ReadonlyVars.ProgramVer;
        }


        public RedditData RedditInfo { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; this.RaiseAndSetIfChanged(ref title, value); }
        }
        
        public IReactiveCommand<Unit> ExitCommand { get; set; }        
        public async Task ExitExcute(object o)
        {
			BoardCollection.SaveBoardCollection(SubredditList.OwnBoardCollection);
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
		public SubredditListViewModel SubredditList { get; set; }

		private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; this.RaiseAndSetIfChanged(ref errorMessage, value); }
        }

		public IReactiveCommand<Unit> AddNewSubredditCommand { get;set; }

		public async Task AddNewSubredditExcute(object sender)
		{
			var w = new NewBoardWindow();
			w.ShowDialog();
			if (w.IsOk)
			{
				var board = new Board()
				{
					BoardName = w.BoardNameText,
					Path = w.UrlText,
					BoardPlace = w.BoardType
				};
                this.SubredditList.OwnBoardCollection.Children.Add(board);
			}
		}

	}
}
