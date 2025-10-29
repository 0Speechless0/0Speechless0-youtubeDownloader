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
        public Dictionary<string, List<string>> SongGroups;
        public UserInfo userinfo;
        public string nextCloudUrl;
        internal object youtubeUserName;

        public DataObject() {
            userinfo    = new UserInfo();
            SongGroups = new Dictionary<string, List<string> >();
            // ListDic = new Dictionary<string, listObject>();
        } 
    }
}
