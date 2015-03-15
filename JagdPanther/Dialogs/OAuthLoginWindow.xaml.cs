using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace JagdPanther.Dialogs
{
	/// <summary>
	/// OAuthLoginWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class OAuthLoginWindow : Window
	{
		private string state;
		public OAuthLoginWindow()
		{
			InitializeComponent();
			Loaded += Window_Loaded;
			state = DateTime.Now.ToString().Replace(" ","");
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			dddd.Navigate("https://www.reddit.com/api/v1/authorize?client_id=gpnimOQbPLFmiw&response_type=code&state=" + state + "&redirect_uri=http://127.0.0.1&duration=permanent&scope=identity,edit,flair,history,modconfig,modflair,modlog,modposts,modwiki,mysubreddits,privatemessages,read,report,save,submit,subscribe,vote,wikiedit,wikiread");
			dddd.Navigated += Dddd_Navigated;
		}

		private void Dddd_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			var str = e.Uri.ToString();
			if (e.Uri.Host.Contains("127.0.0.1"))
			{
				var dic = e.Uri.Query.Split('&').ToList().ToDictionary(x =>
				{
					return x.Split('=')[0].Replace("&","");
				},x =>
				{
					return x.Split('=')[1].Replace("&","");
				});
				var code = dic["code"];

				var wec = new WebClient();
				var ne = new NameValueCollection()
				{
					$grant_type = "authorization_code",
					$code = code,
					$redirect_uri = "http://127.0.0.1"
				};

				wec.Credentials = new NetworkCredential("gpnimOQbPLFmiw", "");
				var data = wec.UploadValues("https://www.reddit.com/api/v1/access_token",ne);
				var jr = JToken.Parse(Encoding.UTF8.GetString(data));
				RefreshToken = jr["refresh_token"].ToString();
				AccessToken = jr["access_token"].ToString();
				this.Close();
            }
		}

		public string RefreshToken { get; set; }

		public string AccessToken { get; set; }
	}
}
