using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	[DataContract]
	public class AccountList
	{
		[DataMember]
		public List<Account> Accounts { get; set; }

		[DataMember]
		public Account LoggedAccount { get;set; }

		[IgnoreDataMember]
		public int Count
		{
			get { return Accounts.Count; }
		}

		public void Save()
		{
			var dcs = new DataContractSerializer(typeof(AccountList));
			using (var fs = File.Open(Folders.LoginInfoXml, FileMode.Open))
			{
				dcs.WriteObject(fs, this);
			}
		}

		public void Load()
		{
			var dcs = new DataContractSerializer(typeof(AccountList));
			using (var fs = File.Open(Folders.LoginInfoXml, FileMode.Open))
			{
				var data = dcs.ReadObject(fs) as AccountList;
				Accounts = data.Accounts;
				LoggedAccount = data.LoggedAccount;
			}
		}
	}
}
