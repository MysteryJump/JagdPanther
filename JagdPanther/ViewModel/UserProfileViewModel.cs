using JagdPanther.Model;
using ReactiveUI;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.ViewModel
{
	public class UserProfileViewModel : ReactiveObject
	{
		public IReactiveCommand<Unit> SetAsAllReadedCommand { get; set; }

		public IReactiveCommand<Unit> MoveToTargetThreadCommand { get;set; }

		public ObservableCollection<Message> UnreadedMessages { get; set; }

		public UserProfileViewModel(RedditData rd)
		{
			var ms = new MessageReader();
			UnreadedMessages = new ObservableCollection<Message>(ms.GetMessage(rd));

			SetAsAllReadedCommand = ReactiveCommand.CreateAsyncTask(SetAsAllReadedExcute);
			MoveToTargetThreadCommand = ReactiveCommand.CreateAsyncTask(MoveToTargetThreadExcute);
			MessageBus.Current.Listen<Message>("SetAsReadComment")
				.Subscribe(x =>
				{
					UnreadedMessages.Remove(x);
					x.BaseMessage.SetAsRead();
				});
		}


		public async Task SetAsAllReadedExcute(object sender)
		{
			UnreadedMessages.ToList()
				.ForEach(x =>
				{
					x.BaseMessage.SetAsRead();
				});
			UnreadedMessages.Clear();
		}

		public async Task MoveToTargetThreadExcute(object sender)
		{
			var ms = sender as Message;
			
		}
    }
}
