using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using youtbue下載介面.Models;

namespace youtbue下載介面.App
{

    public class DownloadProcess
    {
        bool willDownloadPlayList = false;
        CMDAppender _cMDAppender;
        internal DownloadProcess(CMDAppender cMDAppender)
        {
            _cMDAppender = cMDAppender;
        }
        public void downloadOne()
        {
            willDownloadPlayList = false;
            download();
        }

        private void download()
        {
            Console.Write("請輸入下載格式代碼 ( ");

            foreach (DownloadType type in Enum.GetValues(typeof(DownloadType)))
            {
                Console.Write($"{type} => {(int)type}");
            }

            Console.Write(")");

            string downloadType = Console.ReadLine();
            string outputPath = "";

            if (!Enum.TryParse<DownloadType>(downloadType, out DownloadType downloadTypeEnum))
                throw new Exception("無法辨識你輸入的格式");
            else
            {
                if (downloadTypeEnum == DownloadType.Audio)
                {
                    Console.WriteLine("請輸入格式(best/aac/flac/mp3/m4a/opus/vorbis/wav): \n");
                    string format = Console.ReadLine();
                    outputPath = _cMDAppender.AppendOutPutPath(downloadTypeEnum, format);
                }
                else if (downloadTypeEnum == DownloadType.Video)
                {
                    outputPath = _cMDAppender.AppendOutPutPath(downloadTypeEnum);
                }

            }
            if(willDownloadPlayList)
            {
                FileInfo[] fileInfos = new DirectoryInfo(outputPath).GetFiles();
                _cMDAppender.AppendPlayList(fileInfos.Select(e => e.Name).ToArray() );
            }

            Console.WriteLine("請輸入下載連結");
            string url = Console.ReadLine() ?? "" ;
            _cMDAppender.AppendDowndUrl(url);

            _cMDAppender.run();
        }
        
        public void downloadPlayList()
        {
            willDownloadPlayList = true;
            download();
        }
        public void update()
        {
            _cMDAppender.Append("--update-to master");
            _cMDAppender.run();
        }
    }
}