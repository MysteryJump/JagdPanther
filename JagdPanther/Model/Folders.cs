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


    }
}
