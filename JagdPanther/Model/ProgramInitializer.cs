using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	public static class ProgramInitializer
	{
		public static void CheckFolders()
		{
			if (!File.Exists(Folders.CommentListFolder))
				Directory.CreateDirectory(Folders.CommentListFolder);
			if (!File.Exists(Folders.PostListFolder))
				Directory.CreateDirectory(Folders.PostListFolder);
		}
	}
}
