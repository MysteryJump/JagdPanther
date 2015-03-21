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
		private AccountList accounts;

		public ObservableCollection<Account> AccountList
		{
			get
			{
				return accountList;
			}
			set
			{
				accountList = value;
				this.RaiseAndSetIfChanged(ref accountList, value);
			}
		}
		private ObservableCollection<Account> accountList;

		private Account loggedAccount;

		public Account LoggedAccount
		{
			get { return loggedAccount; }
			set { loggedAccount = value; this.RaiseAndSetIfChanged(ref loggedAccount, value); }
		}


		public AccountListViewModel()
		{
			accounts = new AccountList();
			accounts.Load();

			AccountList = new ObservableCollection<Account>();
			accounts.Accounts.ForEach(x => AccountList.Add(x));
			AccountList.Remove(accounts.LoggedAccount);

		}

		
	}
}
