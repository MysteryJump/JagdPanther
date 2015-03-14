using System.Runtime.Serialization;

namespace JagdPanther.Model
{
    [DataContract]
    public enum BoardLocate
    {
		[EnumMember]
        Twoch,
		[EnumMember]
        Jbbs,
		[EnumMember]
        Reddit,
		[EnumMember]
		MReddit
    }
}