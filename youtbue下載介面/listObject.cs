using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    public class listObject
    {
        public string listCode { get; set; }
        public int downloadCount { get; set; } = 0;
        public int lastDownLoadIndex { get; set; } = 1;

        public string listName { get; set; }
        public List<int> startIndexHistory { get; set; } = new List<int>();

    }
}
