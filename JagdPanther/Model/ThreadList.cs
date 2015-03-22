using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	[DataContract]
	public class ThreadList
	{
		[DataMember]
		public List<ThreadForList> Threads { get; set; }

		[DataMember]
		public string Subreddit { get; set; }
		[DataMember]
		public string SubredditPath { get;set; }

		public static explicit operator ThreadList(List<Thread> r)
		{
			var t = new List<ThreadForList>();
			r.ForEach(x =>
			{
				t.Add(new ThreadForList
				{
					CommentCount = x.CommentCount,
					Created = x.CreatedTime,
					Name = x.Title,
					VoteCount = x.VoteCount,
					Id = x.Id,
					Flair = x.Flair
				});
			});
			var th = new ThreadList
			{
				Threads = t
			};
			return th;
		}

		public static explicit operator List<Thread>(ThreadList t)
		{
			var tl = new List<Thread>();
			t.Threads.ForEach(x =>
			{
				tl.Add(new Thread
				{
					CommentCount = x.CommentCount,
					Title = x.Name,
					VoteCount = x.VoteCount,
					CreatedTime = x.Created,
					Id = x.Id,
					Flair = x.Flair
				});
			});
			return tl;
		}

		[DataContract]
		public class ThreadForList
		{
			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public string Id { get; set; }

			[DataMember]
			public DateTime Created { get; set; }

			[DataMember]
			public int CommentCount { get;set; }

			[DataMember]
			public int VoteCount { get; set; }

			[DataMember]
			public string Flair { get; set; }
        }

	}
}
