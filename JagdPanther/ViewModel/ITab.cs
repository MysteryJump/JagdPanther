using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public interface ITab
	{
		IReactiveCommand<Unit> RemoveTabCommand { get; set; }
		IReactiveCommand<Unit> RemoveAllTabCommand { get; set; }

		Task RemoveTabExcute(object sender);
		Task RemoveAllTabExcute(object sender);

		string Title { get;set; }
	}
}
