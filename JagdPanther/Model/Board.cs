using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	[DataContract]
	public class Board
	{
		[DataMember]
		public string BoardName { get; set; }
		[DataMember]
		public string Path { get; set; }
		[IgnoreDataMember]
		public string Description { get; set; }
		[DataMember]
		public BoardLocate BoardPlace { get; set; }

		public IReactiveCommand<Unit> OpenSubredditCommand { get; set; }

		private async Task OpenSubredditExcute(object sender)
		{
			MessageBus.Current.SendMessage(sender.ToString(), "OpenNewSubreddit");
		}

	}
}