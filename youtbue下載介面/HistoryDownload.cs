using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{

    [Serializable]
    public class HistoryDownload
    {
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public int startIndex {get;set;}

        public int endIndex { get; set; }
    }
}
