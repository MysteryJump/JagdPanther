using System;
using RedditSharp.Things;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using System.Threading.Tasks;
using System.Reactive;
using System.Windows;
using System.Diagnostics;

namespace JagdPanther.Model
{
    public class ViewComment
    {
        public ViewComment ThisObject { get { return this; } }
        public int CommentNumber { get; set; }
        public string Body { get; set; }
        public int? ParentAnchor { get; set; }
        public string FlairText { get; set; }
        public string Author { get; set; }
        public string BodyHtml { get; set; }
        public List<ViewComment> Children { get; set; }
        public DateTime Created { get; set; }
        public string Source { get; set; }
        public bool IsGenerator { get { return CommentNumber == 1; } }
        public bool HasBody { get { return (Body != null || Body != String.Empty); } }
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
                ParentPost = (Post)v.Parent,
                BaseComment = v,
                Votes = v.Upvotes - v.Downvotes
            };
            return vc;
        }

        public string AnchorText
        {
            get
            {
                var num = ParentAnchor;
                if (num.HasValue)
                    return ">>"+num;
                else return null;
            }
        }

        public bool IsExistParentAnchor
        {
            get { return ParentAnchor.HasValue; }
        }
        public Comment BaseComment { get; set; }
        public ViewComment Parent { get; internal set; }
        public Post ParentPost { get; set; }

        public IReactiveCommand<Unit> WriteCommentDialogOpenCommand { get; set; }
        public int Votes { get; set; }

        public async Task OpenWriteCommentDialogExcute(object sender)
        {
            var vc = sender as ViewComment;
            if (vc.BaseComment == null)
            {
                var dia = new Dialogs.OpenWriteWindow();

                dia.ShowDialog();
                if (dia.IsClickWriteButton == true)
                    vc.ParentPost.Comment(dia.WriteCommentData);
            }
            else
            {
                var dia = new Dialogs.OpenWriteWindow();

                if (dia.ShowDialog() == true)
                    vc.BaseComment.Reply(dia.WriteCommentData);
            }
        }
        public ViewComment()
        {
            WriteCommentDialogOpenCommand = ReactiveCommand.CreateAsyncTask(OpenWriteCommentDialogExcute);
            VoteCommand = ReactiveCommand.CreateAsyncTask(VoteExcute);
			ReadSourceCommand = ReactiveCommand.CreateAsyncTask(ReadSourceExcute);
			CopyCommentCommand = ReactiveCommand.CreateAsyncTask(CopyCommentExcute);
        }

        public IReactiveCommand<Unit> VoteCommand { get; set; }
        public async Task VoteExcute(object sender)
        {
            dynamic x;
            if (BaseComment == null)
            {
                x = ParentPost;
            }
            else
            {
                x = BaseComment;
            }
            if (sender.ToString() == "UpVote")
            {
                x.Upvote();
            }
            else
            {
                x.Downvote();
            }
        }

		public async Task CopyCommentExcute(object sender)
		{
			var v = sender as ViewComment;
			var com = v.Author + " [" + v.FlairText + "] " + "Vote:" + v.Votes + " " + v.Created + " \r\n"
				 + v.Body;
			Clipboard.SetText(com);
		}
		public IReactiveCommand<Unit> CopyCommentCommand { get;set; }

		public IReactiveCommand<Unit> ReadSourceCommand { get;set; }

		public async Task ReadSourceExcute(object sender)
		{
			Process.Start(Source);
		}

		public IReactiveCommand<Unit> SaveCommentCommand { get; set; }

		public async Task SaveCommentExcute(object sender)
		{
			if (isSavedComment)
			{
				if (IsGenerator)
					ParentPost.Unsave();
				else
					BaseComment.Unsave();
			}
			else
			{
				if (IsGenerator)
					BaseComment.Save();
				else 
					BaseComment.Unsave();
			}
		}

		public bool isSavedComment;

		public string SaveCommentHeader
		{
			get
			{
				if (!IsGenerator)
				{
					isSavedComment = BaseComment.Saved;
				}
				else
				{
					isSavedComment = ParentPost.Saved;
				}
				if (isSavedComment)
					return "保存を解除する";
				else
					return "保存する";
			}
		}


	}
}