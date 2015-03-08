using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
	[DataContract]
	public class BoardCollection
	{
		[DataMember]
		public ObservableCollection<Board> Children { get;set; }

		public static BoardCollection LoadBoardCollection()
		{
			var x = new DataContractSerializer(typeof(BoardCollection));
			try
			{
				using (var y = File.Open(Folders.BoardTreeXml, FileMode.Open))
				{
					return x.ReadObject(y) as BoardCollection;
				}
			}
			catch
			{
				return new BoardCollection() { Children = new ObservableCollection<Board>() };
			}

		}

		public static void SaveBoardCollection(BoardCollection b)
		{
			var x = new DataContractSerializer(typeof(BoardCollection));
			using (var y = File.Open(Folders.BoardTreeXml, FileMode.Create))
			{
				try
				{
					x.WriteObject(y, b);
				}
				catch (Exception e)
				{ throw e; }
			}
		}


	}
}
