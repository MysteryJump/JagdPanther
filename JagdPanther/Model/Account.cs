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
		public string Password { get; set; }
		[IgnoreDataMember]
		public bool? IsLogged { get; set; }

		public static void SaveData(IEnumerable<Account> lists)
		{
			var sb = new StringBuilder();
			var dcs = new DataContractSerializer(typeof(Account));
			foreach (var item in lists)
			{
				var s = new MemoryStream();
				dcs.WriteObject(s, item);
				sb.Append(Encoding.UTF8.GetString(s.GetBuffer())).Append("---------666---------");
				s.Close();
			}
			var c = new Cryptography();
			var dat = c.Encrypt(sb.ToString());
			using (var fs = File.Open(Folders.LoginInfoXml, FileMode.Create))
			{
				fs.Write(dat, 0, dat.Length);
			}
		}
		public static IEnumerable<Account> LoadData()
		{
			var dcs = new DataContractSerializer(typeof(Account));
			var lists = new List<Account>();
			if (!File.Exists(Folders.LoginInfoXml))
				return new List<Account>();
			using (var fs = File.OpenRead(Folders.LoginInfoXml))
			{
				using (var ms = new MemoryStream())
				{
					fs.CopyTo(ms);
					var b = ms.GetBuffer();
					var c = new Cryptography();
					var s = c.Decrypt(b).Split(new[]{ "---------666---------"}, StringSplitOptions.RemoveEmptyEntries);
					foreach (var item in s)
					{
						using (var d = new MemoryStream(Encoding.UTF8.GetBytes(item)))
						{
							try
							{
								lists.Add(dcs.ReadObject(d) as Account);
							}
							catch
							{
								return lists;
							}
						}
					}
				}

			}
			return lists;
		}

		[IgnoreDataMember]
		public IReactiveCommand<Unit> ChangeAccountCommand { get; set; }

		public async Task ChangeAccountExcute(object sender)
		{
			if (IsLogged == false)
				MessageBus.Current.SendMessage(this, "ChangeAccount");
			else if (IsLogged == null)
			{
				var log = new Dialogs.LoginAndGenerateNewUserWindow();
				log.ShowDialog();
				MessageBus.Current.SendMessage(log.Data, "AddAccount");
			}
		}

		public Account()
		{
			ChangeAccountCommand = ReactiveCommand.CreateAsyncTask(ChangeAccountExcute);
		}
    }
}
