using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public interface IThreadListViewer
	{
		ObservableCollection<Thread> ThreadList { get;set; }
		string Name { get;set; }
		string Path { get;set; }
		RedditData RedditInfo { get; set; }
		Thread ListViewSelectedItem { get; set; }
		Task Initializer(string path);
		Task SelectedExcute(object sender);
    }
}
