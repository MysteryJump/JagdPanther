using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public class EditConfigurationViewModel : ReactiveObject
	{
		public IReactiveCommand<Unit> OkCommand { get; set; }
		public IReactiveCommand<Unit> CancelCommand { get; set; }
		public IReactiveCommand<Unit> SelectTreeViewItemCommand { get; set; }


		public async Task OkExcute(object sender)
		{

		}

		public async Task CancelExcute(object sender)
		{

		}

		public async Task SelectTreeViewItemExcute(object sender)
		{

		}
	}
}
