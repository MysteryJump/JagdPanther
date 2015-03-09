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
			MessageBus.Current.Listen<string>("RemoveAllThreadListTab")
				.Subscribe(_ => ThreadListChildrens.Clear());
			ThreadListChildrens = new ObservableCollection<ThreadListViewModel>();
        }

        public ObservableCollection<ThreadListViewModel> ThreadListChildrens { get; set; }
    }
}