using System.Text;
using youtbue下載介面.Models;
using System.Runtime.InteropServices;
namespace youtbue下載介面.App
{

    internal class CMDAppender
    {
        DataObjectHandler _dataObjectHandler;
        CMDCatcher cMDCatcher;
        string userProfile;
        string cmdOptions = "";
        StringBuilder _cmd;
        string listCode= "";

        string _url= "";

        public string os {get;set;}
        listObject appendingListObject;
        public CMDAppender(DataObjectHandler dataObjectHandler, string _os)
        {
            os =_os;
            _cmd = os switch {
                "windows" => new StringBuilder("/C yt-dlp"),
                "linux" =>  new StringBuilder("yt-dlp"),
                _ => new StringBuilder()
            };
            userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _dataObjectHandler = dataObjectHandler;
            appendingListObject = new listObject();

        }
        public void AppendDowndUrl(){
            Console.WriteLine("請輸入下載連結");
            _url = Console.ReadLine() ?? "" ;
            string[] urlArg = _url.getUrlArgs();
            userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            listCode = urlArg.Length > 0 ? urlArg.FirstOrDefault(r => r.Contains("list=") )?.Split('=')[1] : "" ;
            cMDCatcher = new CMDCatcher(listCode);

        }
        public void AppendOutPutPath()
        {
            string format;
            string outputPath="";
            Console.WriteLine("請輸入下載格式(video/audio) :");       
            string downloadType = Console.ReadLine().downLoadTypeCheck() ;
            if(downloadType.Length ==  0 )
                throw new Exception("無法辨識你輸入的格式");
            else
            {
                if (downloadType.Equals("audio"))
                {
                    Console.WriteLine("請輸入格式(best/aac/flac/mp3/m4a/opus/vorbis/wav): \n");
                    format = Console.ReadLine();
                    if( format.Length == 0) throw new Exception("");
                    outputPath = Path.Combine(userProfile, $"Music\\{appendingListObject.dirName}\\{appendingListObject.dirName}-%(title)s.%(ext)s");
                    cmdOptions += ! cmdOptions.Contains(" -x --audio-format ") ? $" -x --audio-format {format} -o \"{outputPath}\"" : "";
                } 
                if(downloadType.Equals("video"))
                {
                    outputPath = Path.Combine(userProfile, $"Videos\\{appendingListObject.dirName}\\{appendingListObject.dirName}-%(title)s.%(ext)s");
                    cmdOptions += ! cmdOptions.Contains(" -o ") ? "  -o \"{outputPath}\"" : "";
                }
                if(!Directory.Exists(Path.GetDirectoryName(outputPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }
        }
        private void AppendPlayListRange()
        {
                        //targetList.lastDownLoadIndex = 0;
            int itemCount = listCode.getPlayListItemCount();   
            Console.WriteLine($"此歌單已下載至第{appendingListObject.lastDownLoadIndex}，下載後續新增{itemCount - appendingListObject.lastDownLoadIndex}首...");
            if(itemCount - appendingListObject.lastDownLoadIndex == 0)
            {
                Console.WriteLine("沒有新歌可以下載");
                return;
            }
            cmdOptions += ! cmdOptions.Contains(" -I ") ? $" -I {appendingListObject.lastDownLoadIndex + 1}::1" : "";
        }
        public void AppendPlayList()
        {

            appendingListObject = _dataObjectHandler.setListObjectByCode(listCode, cMDCatcher.getPlayListName());
            cmdOptions += ! cmdOptions.Contains(" --yes-playlist ") ? " --yes-playlist" : "";
            AppendPlayListRange();

        }

        public void Append(string str)
        {
            cmdOptions += $" {str}"; 
        }
        public StringBuilder GetCMD()
        {
            _cmd.Append($" {cmdOptions}");
            _cmd.Append($" {_url}");
            return _cmd;
        }
    }
}