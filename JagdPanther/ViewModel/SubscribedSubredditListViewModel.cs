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
	public class SubscribedSubredditListViewModel : ReactiveObject
	{
		public SubscribedSubredditListViewModel(RedditData info)
		{
			RedditInfo = info;
			Subreddits = new ObservableCollection<string>();
			SelectedSubredditCommand = ReactiveCommand.CreateAsyncTask(SelectedSubredditExcute);
		}

		public RedditData RedditInfo { get; private set; }

		public ObservableCollection<string> Subreddits { get;set; }

		public async Task Initialize()
		{
			if (MainViewModel.IsOffline)
				return;
			var lists = await Task.Factory.StartNew(() =>
			{
				return RedditInfo.RedditUser.SubscribedSubreddits.ToList();
			});
			lists.ForEach(x => Subreddits.Add(x.DisplayName));

		}

		public IReactiveCommand<Unit> SelectedSubredditCommand { get; set; }

		public async Task SelectedSubredditExcute(object sender)
		{
			MessageBus.Current.SendMessage(new Board { BoardPlace = BoardLocate.Reddit, Path = "/r/" + SelectedItem }, "OpenNewSubreddit");
		}

		private string selectedItem;

		public string SelectedItem
		{
			get { return selectedItem; }
			set { selectedItem = value; this.RaiseAndSetIfChanged(ref selectedItem, value); }
		}
  
	}
}
