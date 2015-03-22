using JagdPanther.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

using System.Text;
using System.Threading.Tasks;
using JagdPanther.Dialogs;
using RedditSharp;
using System.Reactive;

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
		public IReactiveCommand<Unit> AddAccountCommand { get;set; }
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
			AccountList.Remove(AccountList.Where(x => x.RefreshToken == accounts.LoggedAccount.RefreshToken).FirstOrDefault());
			LoggedAccount = accounts.LoggedAccount;
			AddAccountCommand = ReactiveCommand.CreateAsyncTask(AddAccountExcute);

			MessageBus.Current.Listen<Account>("ChangeAccount-1")
				.Subscribe(x =>
				{
					AccountList.Add(LoggedAccount);
					AccountList.Remove(x);
					LoggedAccount = x;
					accounts.LoggedAccount = x;
					accounts.Save();
					MessageBus.Current.SendMessage(x,"ChangeLoggedUser");
				});
		}

		public async Task AddAccountExcute(object sender)
		{
			var s = new OAuthLoginWindow();
			s.ShowDialog();
			var name = new Reddit(OAuthLoginInfo.GetNewAccessToken(s.RefreshToken)).User.Name;
			var ac = new Account() { RefreshToken = s.RefreshToken, UserName = name };
            accounts.Accounts.Add(ac);
			accounts.Save();
			AccountList.Add(ac);
			
		}

	}
}
