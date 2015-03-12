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

namespace JagdPanther.Model
{
    [DataContract]
    public class Thread : ReactiveObject
    {
        [DataMember]
        public DateTime CreatedTime { get; set; }
        [IgnoreDataMember]
        public string CreatedTimeString
        {
            get { return CreatedTime.ToString(CultureInfo.GetCultureInfo("ja-JP")); }
        }
        [IgnoreDataMember]
        public Post PostThread { get; set; }
        [IgnoreDataMember]
        public List<Comment> RawComments
        {
            get { return PostThread.Comments.ToList(); }
        }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int VoteCount { get; set; }
        [IgnoreDataMember]
        private List<ViewComment> sortedComments;
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
            return await Task.Run(() =>
            {
                try
                {
                    var coms = new List<ViewComment>();
                    var queue = new Queue<ViewComment>();
                    var a = PostThread.Author.FullName;
                    var host = new ViewComment()
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
                        Votes = PostThread.Upvotes - PostThread.Downvotes

                    };
                    queue.Enqueue(host);

                    PostThread.Comments.ToList()
                        .ForEach(x =>
                            {
                                var vc = new ViewComment();
                                vc = (ViewComment)x;
                                vc.BasePostAuthor = a;
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

                    return coms.OrderBy(x => x.Created)
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

        public Thread()
        {
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


        public async Task WriteCommentExcute(object sender)
        {
            var pos = new PostingBeforeProcessor(writeText);
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
        }
        [IgnoreDataMember]
		public Color BackgroundColor { get { return Properties.Settings.Default.ThreadViewBackgroundColor; } }
        [IgnoreDataMember]
		public ReactiveCommand<Unit> RemoveAllTabCommand { get; private set; }

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
    }
}
