using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JagdPanther.Model;
using System.IO;
using JagdPanther.ViewModel;
using Newtonsoft.Json.Linq;

namespace JagdPanther.View
{
	public class DesignLoader
	{
		public string Folder { get; set; }
		public DesignLoader(string styleName)
		{
			Folder = Folders.JsonStyleFolder + "\\" + styleName;
		}

		public DesignLoader() : this("default")
		{

		}

		public async void Load()
		{
			var folderList = new List<string>
			{
				"res",
				"respop",
				"tree-res",
				"tree-parent",
				"tab",
				"thread",

			};
			var list = new DesignDictionary();
			await Task.WhenAll(folderList.Select(async x =>
			{
#if DEBUG
				
#endif
				await Task.Factory.StartNew(() =>
				{
#if DEBUG
					lock (this)
					{
#endif
						var lists = new List<DesingProperty>();
						try
						{
							var path = Folder + "\\" + x + ".json";
							var data = File.ReadAllText(path);
							var jk = JsonConvert.DeserializeObject<List<JToken>>(data);
							if (jk == null)
								return;
							jk.ForEach(t =>
							{
								t.ToList().ForEach(xx =>
								{
									lists.Add(new DesingProperty { Name = xx.First.Path, Value = xx.First.ToString().Replace("{", "").Replace("}", "") });
								});
							});
							list.DesignData.Add(x, lists);
						}
						catch (Exception e)
						{ }
#if DEBUG
					}
#endif
				});
			}));
			MainViewModel.DesignJsonData = list;
		}
	}

	public class DesingProperty
	{
		public string Name { get; set; }

		public string Value { get; set; }
	}

	public class DesignDictionary
	{
		public Dictionary<string,List<DesingProperty>> DesignData { get; set; }

		public DesingProperty this[string type, string name]
		{
			get
			{
				if (!DesignData.ContainsKey(type))
					throw new InvalidOperationException();

				var data = DesignData[type];
				return data.First(x => x.Name == name);
			}
		}

		public DesignDictionary()
		{
			DesignData = new Dictionary<string, List<DesingProperty>>();
		}
	}
}
