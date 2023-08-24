using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    [Serializable]
    public class listObject
    {
        public string listCode { get; set; }
        public int downloadCount { get; set; } = 0;
        public int lastDownLoadIndex { get; set; } = 0;
        public DateTime lastUploadTime { get; set; }
        public string dirName {get;set ;}

        public string listName { get; set; }
        public List<int> startIndexHistory { get; set; } = new List<int>();

        public List<string> HistoryListName { get; set; } = new List<string>();


        public List<HistoryDownload> HistoryDownloadList { get; set; } = new List<HistoryDownload>();
    }
}
