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
using System.Windows;
using System.Windows.Threading;

namespace JagdPanther.ViewModel
{
	public class SearchSubredditListViewModel : ReactiveObject
	{
		private string searchText;

		public string SearchText
		{
			get { return searchText; }
			set
			{
				searchText = value;
				SearchSubreddits(value);
				this.RaiseAndSetIfChanged(ref searchText, value);
			}
		}

		private void SearchSubreddits(string value)
		{
			
			lock (this)
			{
				Subreddits.Clear();
				if (string.IsNullOrWhiteSpace(value))
					return;
				Task.Factory.StartNew(() => SubredditSearcher.SearchSubreddit(value))
					.ContinueWith(x =>
				{
					Application.Current.Dispatcher.Invoke(() => x.Result.ForEach(Subreddits.Add));
				});
			}
		}

		public ObservableCollection<string> Subreddits { get; set; }
		private RedditData info;

		public SearchSubredditListViewModel(RedditData info)
		{
			Subreddits = new ObservableCollection<string>();
			this.info = info;
		}

		public IReactiveCommand<Unit> SelectedSubredditCommand { get; set; }

		public async Task SelectedSubredditExcute(object sender)
		{
			MessageBus.Current.SendMessage(new Board { BoardPlace = BoardLocate.Reddit, Path = SelectedItem }, "OpenNewSubreddit");
		}

		private string selectedItem;

		public string SelectedItem
		{
			get { return selectedItem; }
			set { selectedItem = value; this.RaiseAndSetIfChanged(ref selectedItem, value); }
		}

		public SearchSubredditListViewModel()
		{
			SelectedSubredditCommand = ReactiveCommand.CreateAsyncTask(SelectedSubredditExcute);
		}
	}
}
