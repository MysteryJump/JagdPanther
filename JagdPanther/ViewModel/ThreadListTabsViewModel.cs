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
			MessageBus.Current.Listen<object>("RemoveThreadListTab")
				.Subscribe(x => ThreadListChildrens.Remove(x));
			MessageBus.Current.Listen<string>("RemoveAllThreadListTab")
				.Subscribe(_ => ThreadListChildrens.Clear());
			ThreadListChildrens = new ObservableCollection<object>();
        }

		public ObservableCollection<object> ThreadListChildrens { get; set; }

		private object selectedTab;

		public object SelectedTab
		{
			get { return selectedTab; }
			set { selectedTab = value; this.RaiseAndSetIfChanged(ref selectedTab, value); }
		}

	}
}