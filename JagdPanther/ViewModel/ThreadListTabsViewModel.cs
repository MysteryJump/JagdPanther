using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace JagdPanther.ViewModel
{
    public class ThreadListTabsViewModel : ReactiveObject
    {
		public ThreadListTabsViewModel()
		{
			MessageBus.Current.Listen<ThreadListViewModel>("RemoveThreadListTab")
				.Subscribe(x => ThreadListChildrens.Remove(x));
			MessageBus.Current.Listen<MultiSubredditViewModel>("RemoveThreadListTab")
				.Subscribe(x => ThreadListChildrens.Remove(x));

			MessageBus.Current.Listen<string>("RemoveAllThreadListTab")
				.Subscribe(_ => ThreadListChildrens.Clear());
			ThreadListChildrens = new ObservableCollection<IThreadListViewer>();
        }

		public ObservableCollection<IThreadListViewer> ThreadListChildrens { get; set; }

		private IThreadListViewer selectedTab;

		public IThreadListViewer SelectedTab
		{
			get { return selectedTab; }
			set { selectedTab = value; this.RaiseAndSetIfChanged(ref selectedTab, value); }
		}

	}
}