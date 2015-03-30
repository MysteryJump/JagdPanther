using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
    public static class Folders
    {
        public static string BoardTreeXml
        { get; }
        = ReadonlyVars.CurrentFolder + "\\boards.xml";

        public static string LoginInfoXml
        { get; }
        = ReadonlyVars.CurrentFolder + "\\login.xml";

		public static string PostListFolder
		{ get; }
		= ReadonlyVars.CurrentFolder + "\\logs\\posts";

		public static string CommentListFolder
		{ get; }
		= ReadonlyVars.CurrentFolder + "\\logs\\comments";

		public static string ThreadCssForWebView
		{ get; }
		= ThreadStyleFolder + "\\main.css";

		public static string StyleFolder
		{ get; }
		= ReadonlyVars.CurrentFolder + "\\style";

		public static string ThreadStyleFolder
		{ get; }
		= StyleFolder + "\\thread";

		public static string BakaStamp
		{ get; }
		= ThreadStyleFolder + "\\baka.css";

		public static string ThreadViewHtml
		{ get; }
		= ThreadStyleFolder + "\\main.html";

		public static string HeaderHtml
		{ get; }
		= ThreadStyleFolder + "\\header.html";

		public static string FooterHtml
		{ get; }
		= ThreadStyleFolder + "\\footer.html";
    }
}
