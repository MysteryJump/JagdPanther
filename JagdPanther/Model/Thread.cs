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

namespace JagdPanther.Model
{
    public class Thread : ReactiveObject
    {
        public DateTime CreatedTime { get; set; }
        public string CreatedTimeString
        {
            get { return CreatedTime.ToString(CultureInfo.GetCultureInfo("ja-JP")); }
        }

        public Post PostThread { get; set; }

        public List<Comment> RawComments
        {
            get { return PostThread.Comments.ToList(); }
        }
        public string Title { get; set; }
        public int VoteCount { get; set; }

        private List<ViewComment> sortedComments;

        public List<ViewComment> SortedComments
        {
            get
            {
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
        public IReactiveCommand<Unit> RemoveTabCommand { get; set; }
        private async Task<List<ViewComment>> GetAndParseComments()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var coms = new List<ViewComment>();
                    var queue = new Queue<ViewComment>();
                    var host = new ViewComment()
                    {
                        Author = PostThread.Author.FullName,
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

        public IReactiveCommand<Unit> WriteCommentCommand { get; set; }

        private string writeText;

        public string WriteText
        {
            get { return writeText; }
            set { writeText = value; this.RaiseAndSetIfChanged(ref writeText, value); }
        }


        public async Task WriteCommentExcute(object sender)
        {
            var reg = Regex.Match(writeText, @">>(\d+)");
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

		public Color BackgroundColor { get { return Properties.Settings.Default.ThreadViewBackgroundColor; } }

		public ReactiveCommand<Unit> RemoveAllTabCommand { get; private set; }

		public async Task RemoveAllTabExcute(object sender)
		{
			MessageBus.Current.SendMessage("", "RemoveAllThreadTab");
		}
    }
}
