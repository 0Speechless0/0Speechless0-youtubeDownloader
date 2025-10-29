using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面.Models
{
    [Serializable]
    public class listObject
    {
        public string listCode { get; set; }
        public string dirName {get;set ;}

        public string fullDirName {get;set ;}
        public string listName { get; set; }
        public List<int> startIndexHistory { get; set; } = new List<int>();



    }
}
