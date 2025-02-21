using System.Diagnostics;
using System.Text.RegularExpressions;

namespace youtbue下載介面.App
{

    public class CMDCatcher{
        ProcessStartInfo processInfo;
        string _listCode;
        public CMDCatcher(string listCode)
        {
            processInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
            };
            _listCode = listCode;
        }
        private string getPlayListInfo()
        {
            //if (playlistOutput != null) return playlistOutput;
            processInfo.Arguments =  $"/C yt-dlp --flat-playlist https://www.youtube.com/playlist?list={_listCode}";

            Process process = Process.Start(processInfo);
            return process.StandardOutput.ReadToEnd();
        }
        public int getPlayListItemCount() {
            string processOutput = getPlayListInfo();
            return Regex.Matches(processOutput, @"\[download\] Downloading item").Count;
        }
        public string getPlayListName()
        {
            string processOutput = getPlayListInfo();
            return processOutput.Split("[download] Finished downloading playlist: ")[1].Trim();
        }
    }
}