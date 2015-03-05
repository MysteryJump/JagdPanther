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
    public class ThreadTabsViewModel : ReactiveObject
    {
        public ThreadTabsViewModel()
        {
            MessageBus.Current.Listen<Thread>("RemoveTab")
                .Subscribe(x => ThreadTabsChildren.Remove(x));
            ThreadTabsChildren = new ObservableCollection<Thread>();
        }



        public ObservableCollection<Thread> ThreadTabsChildren { get; set; }

    }
}
