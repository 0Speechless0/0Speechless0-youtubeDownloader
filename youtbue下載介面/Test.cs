using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtbue下載介面.Models;
using youtbue下載介面.App;
namespace youtbue下載介面
{
    public class Test
    {
        DataObject dataObject;
        string os ;
        public Test(string _os)
        {
            dataObject = Data.ReadFromBinaryFile<DataObject>(@".\tempData.bin") ?? new DataObject();
            os = _os;
        }
        public void TestDownload()
        {
            string url = Console.ReadLine();
            // var test = new DownloadProcess(new CMDAppender(new DataObjectHandler(), os ));
            // test.download();
        }
        public void TestPlayListDownload()
        {
            string url = Console.ReadLine();
            // var test = new DownloadProcess(new CMDAppender(new DataObjectHandler(), os ));
            // test.downloadPlayList();
        }
    }
}
