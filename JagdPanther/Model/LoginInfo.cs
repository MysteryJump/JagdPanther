using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JagdPanther.Model
{
    [DataContract]
    public class LoginInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Password { get; set; }
        // せんせー、シリアライズする際の暗号化の仕方がわからないっす

        public void SaveData()
        {
            var dcs = new DataContractSerializer(typeof(LoginInfo));
            using (var str = File.Open(Folders.LoginInfoXml, FileMode.Create))
                dcs.WriteObject(str, this);
        }

        public void LoadData()
        {
            LoginInfo li = null;
            var dcs = new DataContractSerializer(typeof(LoginInfo));
            using (var str = File.Open(Folders.LoginInfoXml, FileMode.Open))
                li = dcs.ReadObject(str) as LoginInfo;
            Name = li.Name;
            Password = li.Password;
        }
    }
}
