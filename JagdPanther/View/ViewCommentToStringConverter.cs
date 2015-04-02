using JagdPanther.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Web;
using System.IO;

namespace JagdPanther.View
{
	[ValueConversion(typeof(IEnumerable<ViewComment>), typeof(string))]
	public class ViewCommentToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var v = (value as List<ViewComment>).ToList();
			var sb = new StringBuilder();

			var css = File.Exists(Folders.ThreadCssForWebView) ? File.ReadAllText(Folders.ThreadCssForWebView) : string.Empty;
			var baka = !Properties.Settings.Default.IsShowBakaStamp ? 
				string.Empty : (File.Exists(Folders.BakaStamp) 
				? File.ReadAllText(Folders.BakaStamp) : string.Empty);

			var respopup = File.Exists(Folders.ResPopup) ? File.ReadAllText(Folders.ResPopup) : string.Empty;
			var notifyJs = File.Exists(Folders.NotifyJs) ? File.ReadAllText(Folders.NotifyJs) : string.Empty;

			if (!File.Exists(Folders.HeaderHtml))
				sb.Append(string.Format(@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <title>TypeScript HTML App</title>
    <script>{0}

{1}
</script>
	<style>
{2}
{3}
	</style>
</head>
<body>
    <dl>
",respopup,notifyJs,css,baka));
			else
				sb.Append(string.Format(File.ReadAllText(Folders.HeaderHtml),css,baka));

			var tx = File.Exists(Folders.ThreadViewHtml) ? File.ReadAllText(Folders.ThreadViewHtml) : null;
			
			
			v.ForEach(x => sb.Append(string.Format(tx == null ? BaseString : tx, x.CommentNumber,
				x.FlairText,
				x.Votes,
				x.CreatedString,
				x.Author,
				x.IsExistParentAnchor ? @"<p class=""anchor"">" + x.AnchorText + "</p>" : "",
				HttpUtility.HtmlDecode(x.BodyHtml))));

			sb.Append(File.Exists(Folders.FooterHtml) ? File.ReadAllText(Folders.FooterHtml) : @"    </dl>
</body>
</html>");
			var t = sb.ToString();
			return t;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		public const string BaseString = @"        <dt id=""{0}"">
            <span class=""res"">
                {0}
            </span>
            <span class=""numsplitter"">:</span>
            <span class=""name"">No name</span>
            <span class=""flairsplitter""> [</span>
            <span class=""flair"">{1}</span>
            <span class=""flairsplitter"">] </span>
            <span class=""voteheader"">Vote: </span>
            <span class=""vote"">{2} </span>
            <span class=""dateheader"">日付:</span>
            <span class=""date"">{3} </span>
            <span class=""authorheader"">Author:</span>
            <span class=""author"">{4}</span>
            <span class=""upvote"">UpVote</span>
            &nbsp;
            <span class=""downvote"">DownVote</span>
        </dt>
        <dd class=""body"">
            {5}
            <span class=""main"">
				{6}
            </span>
        </dd>";
    }
}
