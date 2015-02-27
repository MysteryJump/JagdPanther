using System;
using RedditSharp.Things;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JagdPanther.Model
{
    public class ViewComment
    {
        public int CommentNumber { get; set; }
        public string Body { get; set; }
        public int? ParentAnchor { get; set; }
        public string FlairText { get; set; }
        public string Author { get; set; }
        public string BodyHtml { get; set; }
        public List<ViewComment> Children { get; set; }
        public DateTime Created { get; set; }

        public static explicit operator ViewComment(Comment v)
        {
            var lvc = new List<ViewComment>();
            v.Comments.ToList().ForEach(x =>
                {
                    lvc.Add((ViewComment)x);
                });
            var vc = new ViewComment()
            {
                FlairText = v.AuthorFlairText,
                Body = v.Body,
                Author = v.Author,
                BodyHtml = v.BodyHtml,
                Children = lvc,
                Created = v.Created,
            };
            return vc;
        }

        public string AnchorText
        {
            get
            {
                var num = ParentAnchor;
                if (num.HasValue)
                    return $">>{num}";
                else return null;
            }
        }

        public bool IsExistParentAnchor
        {
            get { return ParentAnchor.HasValue; }
        }

        public ViewComment Parent { get; internal set; }
    }
}