using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;
using ReactiveUI;
using System.Reactive;

namespace JagdPanther.Model
{
    public class Thread : ReactiveObject
    {
        public DateTime CreatedTime { get; set; }
        public Post PostThread { get; set; }

        public List<Comment> RawComments
        {
            get { return PostThread.Comments.ToList(); }
        }
        public string Title { get; set; }

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
                int i = 1;
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
            });
        }

        public Thread()
        {
            RemoveTabCommand = ReactiveCommand.CreateAsyncTask(RemoveTabExcute);
        }

        public async Task RemoveTabExcute(object sender)
        {

            MessageBus.Current.SendMessage(this, "RemoveTab");
        }

    }
}
