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
	public class SubredditListViewModel : ReactiveObject
	{
		public BoardCollection OwnBoardCollection
		{
			get { return bc; }
			private set { bc = value;this.RaiseAndSetIfChanged(ref bc, value); }
		}
		private BoardCollection bc;

		public void SetBoardCollection()
		{
			OwnBoardCollection = BoardCollection.LoadBoardCollection();
		}

		public SubredditListViewModel()
		{
			OpenSubredditCommand = ReactiveCommand.CreateAsyncTask(OpenSubredditExcute);
			SetBoardCollection();
		}

		private Board selectedItem;

		public Board SelectedItem
		{
			get { return selectedItem; }
			set { selectedItem = value; this.RaiseAndSetIfChanged(ref selectedItem, value); }
		}

		public IReactiveCommand<Unit> OpenSubredditCommand { get; set; }

		public async Task OpenSubredditExcute(object sender)
		{
			MessageBus.Current.SendMessage(SelectedItem, "OpenNewSubreddit");
		}
	}
}
