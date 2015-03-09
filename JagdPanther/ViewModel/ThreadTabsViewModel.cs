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
            MessageBus.Current.Listen<Thread>("RemoveThreadTab")
                .Subscribe(x => ThreadTabsChildren.Remove(x));
			MessageBus.Current.Listen<string>("RemoveAllThreadTab")
				.Subscribe(_ => ThreadTabsChildren.Clear());

            ThreadTabsChildren = new ObservableCollection<Thread>();
        }



        public ObservableCollection<Thread> ThreadTabsChildren { get; set; }

    }
}
