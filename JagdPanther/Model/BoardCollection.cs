using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.ObjectModel;

namespace JagdPanther.Model
{
    public class BoardCollection : ObservableCollection<Category>
    {

        public static BoardCollection LoadBoards(bool isRef)
        {
            if (!File.Exists(Folders.BoardTreeXml))
                CreateBoardList();
            var dtc = new DataContractSerializer(typeof(SerializableBoardCollection));
			List<Category> bc = null;
            try
            {
                using (var str = File.Open(Folders.BoardTreeXml, FileMode.Open))
					bc = (dtc.ReadObject(str) as SerializableBoardCollection).Categories;
				if (bc == null)
					return new BoardCollection();
            }
            catch { throw new InvalidDataContractException("Invalid data"); }
			return (new ObservableCollection<Category>(bc)) as BoardCollection;
        }

        private static void CreateBoardList()
        {
            SaveBoards(new BoardCollection());
        }

        public static void SaveBoards(BoardCollection bc)
        {
			var s = new SerializableBoardCollection();
			s.Categories = bc.ToList();
            var dtc = new DataContractSerializer(typeof(SerializableBoardCollection));
            using (var str = File.Open(Folders.BoardTreeXml, FileMode.Create))
				dtc.WriteObject(str, s);

        }
		[DataContract]
		public class SerializableBoardCollection
		{
			[DataMember]
			public List<Category> Categories { get; set; }
		}
    }
}
