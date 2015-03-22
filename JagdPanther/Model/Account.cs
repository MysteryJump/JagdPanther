using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReactiveUI;
using System.Reactive;

namespace JagdPanther.Model
{
	[DataContract]
	public class Account
	{
		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public string RefreshToken { get; set; }

		[IgnoreDataMember]
		public IReactiveCommand<Unit> ChangeAccountCommand
		{
			get
			{
				return ReactiveCommand.CreateAsyncTask(ChangeAccountExcute);
			}
		}

		public void GetUserName()
		{
			var rd = RedditControl.Login(RefreshToken);
			UserName = rd.RedditUser.Name;
		}

		public async Task ChangeAccountExcute(object sender)
		{
			MessageBus.Current.SendMessage(this, "ChangeAccount-1");
			MessageBus.Current.SendMessage(this, "ChangeAccount-2");

		}

	}
}
