using JagdPanther.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public class AccountListViewModel : ReactiveObject
	{
		public ObservableCollection<Account> lists;
		public ObservableCollection<Account> Accounts
		{
			get
			{
				return lists;
			}
			set
			{
				lists = value;
				this.RaiseAndSetIfChanged(ref lists, value);
			}
		}

		public AccountListViewModel()
		{
			Accounts = new ObservableCollection<Account>();
			Account.LoadData().ToList().ForEach(Accounts.Add);
			Accounts.Add(new Account() { UserName = "アカウントを追加する", IsLogged = null });

		}

	}
}
