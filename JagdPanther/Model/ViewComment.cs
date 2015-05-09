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
using System.Runtime.Serialization;
using System.Globalization;
using System.Windows.Controls;
using JagdPanther.View;
using System.Windows.Media;
using JagdPanther.ViewModel;

namespace JagdPanther.Model
{
    [DataContract]
	public class ViewComment : ReactiveObject, IViewComment
    {

		[IgnoreDataMember]
		public bool IsFirst { get;set; }
        [IgnoreDataMember]
        public string BasePostAuthor { get; set; }
        [IgnoreDataMember]
        public ViewComment ThisObject { get { return this; } }
        [DataMember]
        public int CommentNumber { get; set; }
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public int? ParentAnchor { get; set; }
        [DataMember]
        public string FlairText { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string BodyHtml { get; set; }
        [IgnoreDataMember]
        public List<ViewComment> Children { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public string Source { get; set; }
		[IgnoreDataMember]
		public bool IsGenerator { get { return BasePostAuthor == Author; } }
        [IgnoreDataMember]
		public bool HasBody { get { return !string.IsNullOrWhiteSpace(Body); } }
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
				Votes = v.Upvotes - v.Downvotes,
				IsFirst = false,
				Id = v.Id
            };
            return vc;
        }
        [IgnoreDataMember]
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
        [IgnoreDataMember]
        public bool IsExistParentAnchor
        {
            get { return ParentAnchor.HasValue; }
        }
        [IgnoreDataMember]
        public Comment BaseComment { get; set; }
        [IgnoreDataMember]
        public ViewComment Parent { get; internal set; }
        [IgnoreDataMember]
        public Post ParentPost { get; set; }
        [IgnoreDataMember]
        public IReactiveCommand<Unit> WriteCommentDialogOpenCommand { get; set; }
        [DataMember]
        public int Votes { get; set; }

        public async Task OpenWriteCommentDialogExcute(object sender)
        {
            var vc = sender as ViewComment;
			var dia = new Dialogs.OpenWriteWindow();
			dia.ShowDialog();
			var data = dia.WriteCommentData;
			var pos = new PostingBeforeProcessor(data);
			pos.ReplaceEndOfLine();

			if (vc.BaseComment == null)
			{
				if (dia.IsClickWriteButton == true)
					vc.ParentPost.Comment(pos.ProcessedText);
			}
            else
            {
				if (dia.IsClickWriteButton == true)
					vc.BaseComment.Reply(pos.ProcessedText);
            }
        }
        public ViewComment()
        {
			ShowParentCommentCommand = ReactiveCommand.CreateAsyncTask(ShowParentCommentExcute);
			WriteCommentDialogOpenCommand = ReactiveCommand.CreateAsyncTask(OpenWriteCommentDialogExcute);
            VoteCommand = ReactiveCommand.CreateAsyncTask(VoteExcute);
			ReadSourceCommand = ReactiveCommand.CreateAsyncTask(ReadSourceExcute);
			CopyCommentCommand = ReactiveCommand.CreateAsyncTask(CopyCommentExcute);
			EditCommand = ReactiveCommand.CreateAsyncTask(EditExcute);
			ShowIdCommentCommand = ReactiveCommand.CreateAsyncTask(ShowIdCommentExcute);
		}
        [IgnoreDataMember]
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
				Votes++;
            }
            else
            {
                x.Downvote();
				Votes--;
            }
        }
        
		public async Task CopyCommentExcute(object sender)
		{
			var v = sender as ViewComment;
			var com = v.Author + " [" + v.FlairText + "] " + "Vote:" + v.Votes + " " + v.Created + " \r\n"
				 + v.Body;
			Clipboard.SetText(com);
		}
        [IgnoreDataMember]
        public IReactiveCommand<Unit> CopyCommentCommand { get;set; }
        [IgnoreDataMember]
        public IReactiveCommand<Unit> ReadSourceCommand { get;set; }

		public async Task ReadSourceExcute(object sender)
		{
			Process.Start(Source);
		}
        [IgnoreDataMember]
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
        [IgnoreDataMember]
        public bool isSavedComment;
        [IgnoreDataMember]
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
		[IgnoreDataMember]
		public IReactiveCommand<Unit> EditCommand { get; set; }

		public async Task EditExcute(object sender)
		{
			var c = new Dialogs.EditTextWindow();
			c.TextBoxText = Body;
			c.ShowDialog();
			try
			{
				var pos = new PostingBeforeProcessor(c.TextBoxText);
				pos.ReplaceEndOfLine();
				BaseComment.EditText(pos.ProcessedText);
			}
			catch (Exception)
			{ 
				MessageBox.Show("編集できませんでした");
			}
		}

        [IgnoreDataMember]
        public string CreatedString
        {
            get { return Created.ToString(CultureInfo.GetCultureInfo("ja-JP")); }
        }
		[IgnoreDataMember]
		public IReactiveCommand<Unit> ShowParentCommentCommand { get;set; }

		public async Task ShowParentCommentExcute(object sender)
		{
			ResponsePopup.ShowParentPopup(sender as TextBlock);
		}

		[DataMember]
		public string Id { get; internal set; }

		public IReactiveCommand<Unit> ShowIdCommentCommand { get;set; }
		public async Task ShowIdCommentExcute(object sender)
		{
			ResponsePopup.ShowIdPopup(sender as TextBlock);
		}

		// design info

		[IgnoreDataMember]
		public string ThreadOwnerHeader { get { return MainViewModel.DesignJsonData["res", "thread-owner-text"].Value; } }

		[IgnoreDataMember]
		public Color ThreadOwnerColor { get { return (Color)ColorConverter.ConvertFromString(MainViewModel.DesignJsonData["res", "thread-owner-color"].Value); } }
    }
}