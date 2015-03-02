using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;

namespace JagdPanther.Model
{
    public class Thread
    {
        public DateTime CreatedTime { get; set; }
        public Post PostThread { get; set; }

        public List<Comment> RawComments
        {
            get { return PostThread.Comments.ToList(); }
        }
        public string Title { get; set; }

        public List<ViewComment> SortedComments { get { return GetAndParseComments(); } }

        private List<ViewComment> GetAndParseComments()
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
                ParentPost = PostThread

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
        }
    }
}
