using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	[DataContract]
	public class OAuthLoginInfo
	{
		[DataMember]
		public string RefreshToken { get; set; }

		public void SaveData()
		{
			var dcs = new DataContractSerializer(typeof(OAuthLoginInfo));
			using (var str = File.Open(Folders.LoginInfoXml, FileMode.Create))
				dcs.WriteObject(str, this);
		}

		internal string GetNewAccessToken()
		{
			var wec = new WebClient();
			wec.Credentials = new NetworkCredential("gpnimOQbPLFmiw" ,"");
			var n = new NameValueCollection()
			{
				$grant_type = "refresh_token",
				$refresh_token = RefreshToken
            };
			var x = wec.UploadValues("https://www.reddit.com/api/v1/access_token", n);
			return JToken.Parse(Encoding.UTF8.GetString(x))["access_token"].ToString();
		}

		public void LoadData()
		{
			OAuthLoginInfo li;
			var dcs = new DataContractSerializer(typeof(OAuthLoginInfo));
			using (var str = File.Open(Folders.LoginInfoXml, FileMode.Open))
				li = dcs.ReadObject(str) as OAuthLoginInfo;
			RefreshToken = li.RefreshToken;
		}
	}
}
