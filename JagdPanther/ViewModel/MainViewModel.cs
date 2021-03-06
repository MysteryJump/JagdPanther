﻿using JagdPanther.Dialogs;
using JagdPanther.Model;
using JagdPanther.View;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace JagdPanther.ViewModel
{
    public class MainViewModel : ReactiveObject
    {
		public MainViewModel()
		{
			RedditSharp.WebAgent.UserAgent = ReadonlyVars.UserAgent;
#if !DEBUG 
			ProgramInitializer.LoginCheck();
#endif
			AccountList = new AccountListViewModel();
			var a = AccountList.LoggedAccount;
			RedditInfo = RedditControl.Login(a.RefreshToken);
			if (RedditInfo != null)
				a.UserName = RedditInfo.RedditUser.Name;
			else
				IsOffline = true;
			var t = new Timer();
			t.Interval = 2400 * 1000;
			if (!IsOffline)
				ProgramInitializer.DownloadBakaStamp();
			t.Elapsed += (sender, e) =>
			{
				lock (this)
				{
					if (!IsOffline)
						RedditInfo = RedditControl.Login(AccountList.LoggedAccount.RefreshToken);
					MessageBus.Current.SendMessage(RedditInfo, "ChangeAccount-3");
				}
			};
			t.Start();
			if (RedditInfo == null)
				IsOffline = true;

			RegisterCommands();
			InitilizeViewModels();
			RegisterMessageListener();
			var ds = new DesignLoader();
			ds.Load();
			Title = ReadonlyVars.ProgramName + " " + ReadonlyVars.ProgramVer;

		}

		private void InitilizeViewModels()
		{
			ThreadListTabs = new ThreadListTabsViewModel();
			SubredditList = new SubredditListViewModel();
			ThreadTabs = new ThreadTabsViewModel();
			SubscribedSubredditList = new SubscribedSubredditListViewModel(RedditInfo);
			SearchSubredditList = new SearchSubredditListViewModel(RedditInfo);
		}

		private void RegisterMessageListener()
		{
			MessageBus.Current.Listen<Board>("OpenNewSubreddit").Subscribe(async (x) =>
			{
				try
				{
					IThreadListViewer v = null;
					if (x == null)
						return;

					if (x.BoardPlace == BoardLocate.Reddit)
					{
						v = new ThreadListViewModel();
						v.RedditInfo = RedditInfo;

						await v.Initializer(x.Path,IsOffline);
						ThreadListTabs.ThreadListChildrens.Add(v);
						ThreadListTabs.SelectedTab = v;
					}
					else if (x.BoardPlace == BoardLocate.MReddit)
					{
						var path = x.Path;
						if (path.StartsWith("/"))
							path = path.Remove(0, 1);
						v = new MultiSubredditViewModel();
						
						v.RedditInfo = RedditInfo;

						await v.Initializer(path,IsOffline);
						if (v.ThreadList.Count == 0)
							return;
						ThreadListTabs.ThreadListChildrens.Add(v);
						ThreadListTabs.SelectedTab = v;

					}
					MessageBus.Current.SendMessage(v, "ThreadListSelectTab");

				}
				catch (Exception e)
				{
					System.Windows.MessageBox.Show("板のURLが間違っている可能性があります\r\n" + e);
				}
			});
			MessageBus.Current.Listen<Thread>("OpenNewThreadTab").Subscribe(x =>
				{
					ThreadTabs.ThreadTabsChildren.Add(x);
					MessageBus.Current.SendMessage(x, "ThreadSelectTab");
				});

			MessageBus.Current.Listen<Account>("ChangeAccount-2").Subscribe(x =>
			{
				RedditInfo = RedditControl.Login(x.RefreshToken);
				MessageBus.Current.SendMessage(RedditInfo, "ChangeAccount-3");
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
			ChangeViewStateCommand = ReactiveCommand.CreateAsyncTask(ChangeViewStateExcute);
			OpenCommand = ReactiveCommand.CreateAsyncTask(OpenExcute);
			ChangeBoardTreeVisibilityCommand = ReactiveCommand.CreateAsyncTask(ChangeBoardTreeVisibilityExcute);
			OpenInboxCommand = ReactiveCommand.CreateAsyncTask(OpenInboxExcute);
		}
		
        public RedditData RedditInfo
		{
			get { return redditInfo; }
			set
			{
				redditInfo = value;
			}
		}

		private RedditData redditInfo;

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; this.RaiseAndSetIfChanged(ref title, value); }
        }
        
        public IReactiveCommand<Unit> ExitCommand { get; set; }
		private async Task ExitExcute(object o)
        {
			var ds = new WindowStateSaver(this);
			BoardCollection.SaveBoardCollection(SubredditList.OwnBoardCollection);
			if (Properties.Settings.Default.IsSaveThreadListView)
				ds.SaveThreadListTabs();
			if (Properties.Settings.Default.IsSaveThreadView)
				ds.SaveThreadTabs();
			System.Windows.Application.Current.Shutdown();
        }

        public IReactiveCommand<Unit> OpenLicenseWindowCommand { get; set; }

        private async Task OpenLicenseWindowExcute(object sender)
        {
            var w = new LicenseInfo();
            w.ShowDialog();
        }
        public IReactiveCommand<Unit> OpenVersionWindowCommand { get; set; }

		private async Task OpenVersionWindowExcute(object sender)
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

		private async Task AddNewSubredditExcute(object sender)
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
		private async Task RemoveSubredditExcute(object sender)
		{
			SubredditList.OwnBoardCollection.Children.Remove(SubredditList.SelectedItem);
		}

		public IReactiveCommand<Unit> OpenSettingWindowCommand { get;set; }

		private async Task OpenSettingWindowExcute(object sender)
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

		public Orientation RightColumnOriented
		{
			get { return rightColumnOriented; }
			set { rightColumnOriented = value; this.RaiseAndSetIfChanged(ref rightColumnOriented, value); }
		}

		private Orientation rightColumnOriented;


		public string ViewStateText
		{
			get
			{
				return "見た目を変える";
			}
		}

		public IReactiveCommand<Unit> ChangeViewStateCommand { get; set; }
		public SubscribedSubredditListViewModel SubscribedSubredditList { get; private set; }
		public IReactiveCommand<Unit> OpenCommand { get; private set; }
		public SearchSubredditListViewModel SearchSubredditList { get; private set; }

		private async Task ChangeViewStateExcute(object sender)
		{
			RightColumnOriented = RightColumnOriented == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			MessageBus.Current.SendMessage(RightColumnOriented, "ChangeViewState");
		}

		private async Task OpenExcute(object sender)
		{
			var ds = new WindowStateSaver(this);
			IsOffline = true;
			if (Properties.Settings.Default.IsSaveThreadListView)
				ds.LoadThreadListTabs();
			if (Properties.Settings.Default.IsSaveThreadView)
				ds.LoadThreadTabs();
			IsOffline = false;
			await SubscribedSubredditList.Initialize();
		}

		public static bool IsOffline { get; set; }
		public IReactiveCommand<Unit> ChangeBoardTreeVisibilityCommand { get; private set; }

		private async Task ChangeBoardTreeVisibilityExcute(object sender)
		{
			MessageBus.Current.SendMessage("", "BoardTreeWidth");
		}

		private async Task OpenInboxExcute(object sender)
		{
			var us = new UserProfileWindow();
			us.DataContext = new UserProfileViewModel(RedditInfo);
			us.ShowDialog();
		}
		public IReactiveCommand<Unit> OpenInboxCommand { get; set; }

		// i hate stateic public property
		public static DesignDictionary DesignJsonData { get; internal set; }
    }
}
