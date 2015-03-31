using RedditSharp.Things;
using System;

namespace JagdPanther.Model
{
	public interface IViewComment
	{
		string Author { get; set; }
		string FlairText { get; set; }
		string BodyHtml { get; set; }
		int Votes { get; set; }
		string BasePostAuthor { get; set; }
		string Source { get; set; }
		bool IsGenerator { get; }
		Comment BaseComment { get; set; }
		DateTime Created { get; set; }
		string CreatedString { get; }
	}
}