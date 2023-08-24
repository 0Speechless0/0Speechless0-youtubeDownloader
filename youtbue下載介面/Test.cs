using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    public static class Test
    {
        internal static void init(DataObject data, string code)
        {
            var _listName = data.ListDic[code].listName;
            data.ListDic[code] = new listObject() { 
                listName = code.getPlayListName()
            };
            Data.WriteToBinaryFile(@".\tempData.bin", data);

        }
    }
}
