using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面.Models
{
    [Serializable]
    internal class DataObject
    {

        public Dictionary<string, listObject> ListDic;
        public UserInfo userinfo;
        public string nextCloudUrl;

        public DataObject() { 
            userinfo = new UserInfo();
            ListDic = new Dictionary<string, listObject>();
        } 
    }
}
