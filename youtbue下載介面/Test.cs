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
        public Test()
        {
            dataObject = Data.ReadFromBinaryFile<DataObject>(@".\tempData.bin") ?? new DataObject();

        }
        public void TestDownload()
        {
            string url = Console.ReadLine();
            var test = new DownloadProcess(new CMDAppender(dataObject, url.getUrlArgs()));
            test.download(url);
        }
        public void TestPlayListDownload()
        {
            string url = Console.ReadLine();
            var test = new DownloadProcess(new CMDAppender(dataObject, url.getUrlArgs()));
            test.downloadPlayList(url);
        }
    }
}
