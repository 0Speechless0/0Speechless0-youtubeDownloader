using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    internal class DataObjectHandler
    {
        DataObject DataObject { get; set; }
        public KeyValuePair<string, listObject>[] ListObjectArr { get; }

        public DataObjectHandler(DataObject dataObject)
        {
            DataObject = dataObject;
            ListObjectArr = DataObject.ListDic.ToArray();
        }
        
        public List<string> showListName()
        {
            int i = 0;
            return DataObject.ListDic.Select(row => $"({++i})[{row.Value.listName}]").ToList();
        }

        public listObject GetListObject(int index)
        {
            return ListObjectArr[index - 1].Value;
        }
        public List<string> showListHistory(int index)
        {
            int i = 0;
            return ListObjectArr[index -1].Value
                .HistoryDownloadList
                .Select(row => $"({++i})({row.Name})[{row.CreateTime}]").ToList();
        }
    }
}
