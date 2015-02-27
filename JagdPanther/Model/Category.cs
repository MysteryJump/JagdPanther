using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace JagdPanther.Model
{
    [DataContract]
    public class Category
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ObservableCollection<Board> Boards { get; set; }
    }
}