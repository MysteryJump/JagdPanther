using System.Runtime.Serialization;

namespace JagdPanther.Model
{
    [DataContract]
    public class Board
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public BoardLocate BoardType { get; set; }
    }
}