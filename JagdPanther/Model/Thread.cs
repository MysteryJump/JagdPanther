using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;
using ReactiveUI;
using System.Reactive;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.IO;
using JagdPanther.ViewModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JagdPanther.Model
{
    [DataContract]
	public class Thread : ReactiveObject, ITab
    {
		[DataMember]
		public string SubredditName { get; set; }
        [DataMember]
        public DateTime CreatedTime { get; set; }
        [IgnoreDataMember]
        public string CreatedTimeString
        {
            get { return CreatedTime.ToString(CultureInfo.GetCultureInfo("ja-JP")); }
        }
		[IgnoreDataMember]
		public double Speed
		{
			get
			{
				if (CreatedTime == DateTime.MinValue)
					return 0;
				var now = DateTimeOffset.Now.AddHours(9).ToUnixTimeSeconds();
				var created = new DateTimeOffset(CreatedTime).ToUnixTimeSeconds();
				return Math.Truncate(86400.0 * CommentCount / (now - created)*10) / 10;
			}
		}

        [IgnoreDataMember]
        public Post PostThread { get; set; }
        [IgnoreDataMember]
        public List<Comment> RawComments
        {
            get
			{

				//if (rawComments == null)
				return PostThread.Comments.ToList();
				//return rawComments;
			}
        }
		[IgnoreDataMember]
		private List<Comment> rawComments;
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int VoteCount { get; set; }
        [IgnoreDataMember]
		private List<ViewComment> sortedComments { get;set; }
        [DataMember]
        public List<ViewComment> SortedComments
        {
            get
            {
                if (CommentCount == -1)
                    return null;
				if (sortedComments == null)
                    Task.Factory.StartNew(SubscribeComments).Wait();
                return sortedComments;
            }
            private set
            {
                sortedComments = value;
            }

        }

        public async Task SubscribeComments()
        {
            sortedComments = await GetAndParseComments();
        }
        [IgnoreDataMember]
        public IReactiveCommand<Unit> RemoveTabCommand { get; set; }
		private async Task<List<ViewComment>> GetAndParseComments()
		{
			if (!MainViewModel.IsOffline)
			{
				return await Task.Run(() =>
				{
					try
					{
						ReadedItemsCount = CommentCount;
						var coms = new List<ViewComment>();
						var queue = new Queue<ViewComment>();
						SubredditName = PostThread.Subreddit;
						var host = CreateFirst();
						queue.Enqueue(host);

						RawComments.ToList()
							.ForEach(x =>
						{
							var vc = new ViewComment();
							vc = (ViewComment)x;
							vc.BasePostAuthor = host.BasePostAuthor;
							queue.Enqueue(vc);
						});
						var i = 1;
						while (true)
						{
							if (queue.Count == 0)
							{
								break;
							}
							var co = queue.Dequeue();

							co.Children.ForEach(x =>
							{
								x.Parent = co;
								queue.Enqueue(x);
							});
							coms.Add(co);
						}

						var dcs = new DataContractSerializer(typeof(Thread));
						using (var fs = File.Open(Folders.CommentListFolder + "\\" + PostThread.Id + "-" + PostThread.Subreddit + ".xml", FileMode.Create))
							dcs.WriteObject(fs, this);
						return coms.OrderByDescending(x => x.IsFirst)
							.ThenBy(x => x.Created)
							.Where(x => x.Created != new DateTime() || x.ParentAnchor == 0)
							.Select(x =>
						{
							if (x.Parent != null)
								x.ParentAnchor = x.Parent.CommentNumber;
							x.CommentNumber = i;
							i++;
							return x;
						})
							.ToList();
					}
					catch (WebException w)
					{
						MessageBus.Current.SendMessage(w.ToString(), "ErrorMessage");
						return null;
					}
				});
			}
			else
			{
				var p = Folders.CommentListFolder + "\\" + Id + "-" + SubredditName + ".xml";
				if (!File.Exists(p))
				{
					MessageBus.Current.SendMessage("Cannot open file: doesn't exist file", "ErrorMessage");
					return null;
				}
				using (var fs = File.Open(p, FileMode.Open))
				{
					var dcs = new DataContractSerializer(typeof(Thread));
					var th = dcs.ReadObject(fs) as Thread;
					return th.SortedComments;

				}


			}

		}

        public Thread()
        {
            IsEnableWrite = true;
            RemoveTabCommand = ReactiveCommand.CreateAsyncTask(RemoveTabExcute);
            WriteCommentCommand = ReactiveCommand.CreateAsyncTask(WriteCommentExcute);
			RemoveAllTabCommand = ReactiveCommand.CreateAsyncTask(RemoveAllTabExcute);
		}

		public async Task RemoveTabExcute(object sender)
        {

            MessageBus.Current.SendMessage(this, "RemoveThreadTab");
        }
        [IgnoreDataMember]
        public IReactiveCommand<Unit> WriteCommentCommand { get; set; }
        [IgnoreDataMember]
        private string writeText;
        [IgnoreDataMember]
        public string WriteText
        {
            get { return writeText; }
            set { writeText = value; this.RaiseAndSetIfChanged(ref writeText, value); }
        }
		[IgnoreDataMember]
		public bool IsEnableWrite
		{
			get { return isEnableWrite; }
			set { isEnableWrite = value; this.RaiseAndSetIfChanged(ref isEnableWrite, value); }
		}
		[IgnoreDataMember]
		private bool isEnableWrite;

        private async Task WriteCommentExcute(object sender)
        {
			try {
				var pos = new PostingBeforeProcessor(WriteText);
				pos.ReplaceEndOfLine();

				var reg = Regex.Match(pos.ProcessedText, @">>(\d+)");
				var value = reg.Groups[1].Value;
				if (value != "")
				{
					var anc = int.Parse(value);
					SortedComments[anc - 1].BaseComment.Reply(Regex.Replace(writeText, @">>(\d+)", ""));
				}
				else
				{
					PostThread.Comment(writeText);
				}
				WriteText = "";
			}
			catch
			{
				MessageBox.Show("書き込みに失敗しました");
			}
        }
		// design info
        [IgnoreDataMember]
		public Color BackgroundColor { get { return (Color)ColorConverter.ConvertFromString(MainViewModel.DesignJsonData["thread","background-color"].Value); } }

		[IgnoreDataMember]
		public bool IsUseBackgroundImage { get { return Convert.ToBoolean(MainViewModel.DesignJsonData["thread", "is-use-background-image"].Value); } }

		[IgnoreDataMember]
		public string BackgroundImagePath { get { return MainViewModel.DesignJsonData["thread", "background-image-path"].Value; } }

		[IgnoreDataMember]
		public Brush BackgroundData
		{
			get
			{
				Brush b = null;
				if (IsUseBackgroundImage)
				{
					var img = new BitmapImage();
					img.BeginInit();
					img.CacheOption = BitmapCacheOption.OnLoad;
					img.CreateOptions = BitmapCreateOptions.None;
					img.UriSource = new Uri(BackgroundImagePath);
					img.EndInit();
					img.Freeze();
					b = new ImageBrush(img);
				}
				else
				{
					b = new SolidColorBrush(BackgroundColor);
				}
				return b;
			}
		}

		// end design info

		[IgnoreDataMember]
		public IReactiveCommand<Unit> RemoveAllTabCommand { get; set; }

		public async Task RemoveAllTabExcute(object sender)
		{
			MessageBus.Current.SendMessage("", "RemoveAllThreadTab");
		}
        [IgnoreDataMember]
        private int writingBoxHeight;
        [IgnoreDataMember]
        public int WritingBoxHeight
        {
            get { return writingBoxHeight; }
            set { writingBoxHeight = value; this.RaiseAndSetIfChanged(ref writingBoxHeight, value); Properties.Settings.Default.WritingPlaceHeight = value; }
        }
        [DataMember]
        public int CommentCount { get; internal set; }
		[IgnoreDataMember]
		public string Id { get;set; }

		public static Thread LoadLog(string path)
		{
			var dcs = new DataContractSerializer(typeof(Thread));
			Thread th = null;
			using (var f = File.Open(Folders.CommentListFolder + "\\" + path + ".xml", FileMode.Open))
				th = (Thread)dcs.ReadObject(f);
			return th;
		}
		[DataMember]
		public string Flair { get;set; }

		[DataMember]
		public ViewComment ReadedItem { get; set; } // for itemscontrol

		[DataMember]
		public int ReadedScrollHeight { get; set; } // for webview

		public void Save()
		{
			var dcs = new DataContractSerializer(typeof(Thread));
			using (var fs = File.Open(Folders.CommentListFolder + "\\" + PostThread.Id + "-" + PostThread.Subreddit + ".xml", FileMode.Create))
				dcs.WriteObject(fs, this);
		}

		[DataMember]
		public int ReadedItemsCount { get;set; }

		[IgnoreDataMember]
		public int NewItemsCount { get { return CommentCount - ReadedItemsCount; } }

		[IgnoreDataMember]
		public List<TreeViewComment> TreeComment
		{
			get
			{
				var list = new List<TreeViewComment>();
				RawComments.ForEach(x =>
				{
					TreeViewComment cx = (TreeViewComment)x;
					list.Add(cx);
				});
				return list;
			}
		}

		[IgnoreDataMember]
		public ViewComment FirstComment
		{
			get
			{
				return CreateFirst();
			}
		}

		private ViewComment CreateFirst()
		{
			var a = PostThread.Author.FullName;

			return new ViewComment()
			{
				Author = a,
				BasePostAuthor = a,
				ParentAnchor = null,
				Body = PostThread.SelfText,
				BodyHtml = PostThread.SelfTextHtml,
				FlairText = PostThread.AuthorFlairText,
				Children = new List<ViewComment>(),
				Created = PostThread.Created,
				Source = PostThread.Url.ToString(),
				ParentPost = PostThread,
				Votes = PostThread.Upvotes - PostThread.Downvotes,
				IsFirst = true,
				Id = PostThread.Id

			};
		}
	}
}
