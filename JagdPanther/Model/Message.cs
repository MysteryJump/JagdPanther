using ReactiveUI;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	public class Message
	{
		public string MessageTypeString
		{
			get
			{
				return MessageType.ToString();
			}
		}
		public MessageType MessageType { get; set; }

		public string Place { get; set; }

		public DateTime Created { get; set; }

		public string CreatedString
		{
			get
			{
				return Created.ToString(CultureInfo.GetCultureInfo("ja-jp"));
			}
		}

		public string Body { get; set; }

		public Comment BaseMessage { get; set; }
		public IReactiveCommand<Unit> SetAsReadedCommand { get; set; }

		private async Task SetAsReadedExcute(object sender)
		{
			var ms = sender as Message;
			MessageBus.Current.SendMessage(ms, "SetAsReadComment");
		}
		public Message()
		{
			SetAsReadedCommand = ReactiveCommand.CreateAsyncTask(SetAsReadedExcute);
		}
	}

	public class MessageReader
	{
		public List<Message> GetMessage(RedditData rd)
		{
			var mes = new List<Message>();
			rd.RedditUser.UnreadMessages.ToList()
				.ForEach(x =>
				{
					var s = x as Comment;
					mes.Add(new Message
					{
						Body = s.Body,
						Place = s.Subreddit,
						Created = s.Created,
						MessageType = MessageType.Comment,
						BaseMessage = s,
					});
				});
			return mes;
		}
	}
}
