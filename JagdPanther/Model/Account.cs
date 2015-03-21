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
    }
}
