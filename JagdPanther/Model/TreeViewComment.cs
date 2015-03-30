using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp.Things;

namespace JagdPanther.Model
{
	public class TreeViewComment : IViewComment
	{
		public List<TreeViewComment> Children { get; set; }

		public string Author { get; set; }

		public string FlairText { get; set; }

		public string BodyHtml { get; set; }

		public int Votes { get; set; }

		public string BasePostAuthor { get; set; }

		public string Source { get; set; }

		public bool IsGenerator
		{
			get
			{
				return BasePostAuthor == Author;
			}
		}

		public Comment BaseComment { get; set; }

		public static explicit operator TreeViewComment(Comment c)
		{

			var children = new List<TreeViewComment>();
			c.Comments.ToList()
				.ForEach(x => children.Add((TreeViewComment)x));
			return new TreeViewComment
			{
				Author = c.Author,
				FlairText = c.AuthorFlairText,
				BodyHtml = c.BodyHtml,
				BaseComment = c,
				Votes = c.Upvotes - c.Downvotes,
				Children = children,
			};
		}
    }
}
