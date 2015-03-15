using JagdPanther.Dialogs;
using JagdPanther.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace JagdPanther.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
			RedditInfo = RedditControl.Login();
			var t = new Timer();
			t.Interval = 1800 * 1000;
			t.Elapsed += (sender, e) =>
			{
				RedditInfo = RedditControl.Login();
			};
			if (RedditInfo == null)
			{
				App.Current.Shutdown();
			}
			else
			{
				RegisterCommands();
				InitilizeViewModels();
				RegisterMessageListener();
				Title = ReadonlyVars.ProgramName + " " + ReadonlyVars.ProgramVer;
				//Account a = null;
				//AccountList.Accounts.ToList().ForEach(x =>
				//{
				//	if (x.IsLogged == true)
				//		a = x;
				//});
				//if (a != null)
				//{
				//	RedditInfo = RedditControl.Login(a);
				//}
			}
		}

		private void InitilizeViewModels()
		{
			ThreadListTabs = new ThreadListTabsViewModel();
			SubredditList = new SubredditListViewModel();
			ThreadTabs = new ThreadTabsViewModel();
			//AccountList = new AccountListViewModel();
		}

		private void RegisterMessageListener()
		{
			MessageBus.Current.Listen<Board>("OpenNewSubreddit").Subscribe(async (x) =>
			{
				try
				{
					if (x == null)
						return;
					if (x.BoardPlace == BoardLocate.Reddit)
					{
						var v = new ThreadListViewModel();
						v.RedditInfo = RedditInfo;
						await v.Initializer(x.Path);
						ThreadListTabs.ThreadListChildrens.Add(v);

					}
					else if (x.BoardPlace == BoardLocate.MReddit)
					{
						var v = new MultiSubredditViewModel();
						var path = x.Path;
						if (path.StartsWith("/"))
							path = path.Remove(0, 1);
						v.RedditInfo = RedditInfo;

						await v.Initializer(x.Path);
						ThreadListTabs.ThreadListChildrens.Add(v);
					}
				}
				catch
				{
					System.Windows.MessageBox.Show("板のURLが間違っている可能性があります");
				}
			});
			MessageBus.Current.Listen<Thread>("OpenNewThreadTab").Subscribe(x =>
				{
					ThreadTabs.ThreadTabsChildren.Add(x);
				});
			MessageBus.Current.Listen<string>("ErrorMessage").Subscribe(x =>
				{
					ErrorMessage = x;
				});
			MessageBus.Current.Listen<Account>("ChangeAccount").Subscribe((x) =>
			{
				RedditInfo = RedditControl.Login(x);
				AccountList.Accounts.Where(y => y.IsLogged == true).FirstOrDefault().IsLogged = false;
				AccountList.Accounts.Where(y => y == x).FirstOrDefault().IsLogged = true;
            });
			MessageBus.Current.Listen<Account>("AddAccount").Subscribe((x) =>
			{
				AccountList.Accounts.Add(x);
			});
		}

        private void RegisterCommands()
        {
			RemoveSubredditCommand = ReactiveCommand.CreateAsyncTask(RemoveSubredditExcute);
            ExitCommand = ReactiveCommand.CreateAsyncTask(ExitExcute);
			OpenSettingWindowCommand = ReactiveCommand.CreateAsyncTask(OpenSettingWindowExcute);
			AddNewSubredditCommand = ReactiveCommand.CreateAsyncTask(AddNewSubredditExcute);
            OpenLicenseWindowCommand = ReactiveCommand.CreateAsyncTask(OpenLicenseWindowExcute);
            OpenVersionWindowCommand = ReactiveCommand.CreateAsyncTask(OpenVersionWindowExcute);
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

		public IReactiveCommand<Unit> RemoveSubredditCommand { get;set; }
		public async Task RemoveSubredditExcute(object sender)
		{
			SubredditList.OwnBoardCollection.Children.Remove(SubredditList.SelectedItem);
		}

		public IReactiveCommand<Unit> OpenSettingWindowCommand { get;set; }

		public async Task OpenSettingWindowExcute(object sender)
		{
			var r = new SettingWindow();
			r.ShowDialog();
		}

        private int windowHeight;

        public int WindowHeight
        {
            get { return windowHeight; }
            set { windowHeight = value; this.RaiseAndSetIfChanged(ref windowHeight, value); Properties.Settings.Default.WindowHeight = value; }
        }

        private int windowWidth;

        public int WindowWidth
        {
            get { return windowWidth; }
            set { windowWidth = value; this.RaiseAndSetIfChanged(ref windowWidth, value); Properties.Settings.Default.WindowWidth = value; }
        }

		public AccountListViewModel AccountList { get; private set; }
	}
}
