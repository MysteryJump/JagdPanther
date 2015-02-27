using ReactiveUI;
using System.Collections.ObjectModel;

namespace JagdPanther.ViewModel
{
    public class ThreadListTabsViewModel : ReactiveObject
    {
        public ThreadListTabsViewModel()
        {
            ThreadListChildrens = new ObservableCollection<ThreadListViewModel>();
        }

        public ObservableCollection<ThreadListViewModel> ThreadListChildrens { get; set; }
    }
}