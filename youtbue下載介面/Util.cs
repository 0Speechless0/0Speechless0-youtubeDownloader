using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    internal static class Util 
    {
        public static int UrlTypeCheck(this string urlEnd) => urlEnd switch
        {
            "watch" => 1,
            "playlist" => 2,
            _ => 0
        };
        public static string downLoadTypeCheck(this string downloadType) => downloadType switch
        {
            "video" => "video",
            "audio" => "audio",
            _ => ""
        };


        public static int getPlayListItemCount(this string listCode)
        {
            string output = getPlayListInfo(listCode);
            return Regex.Matches(output, @"\[download\] Downloading item").Count;
        }

        static string playlistOutput = null;
        static string getPlayListInfo(string listCode) {
            //if (playlistOutput != null) return playlistOutput;
            var p = System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $"/C yt-dlp --flat-playlist https://www.youtube.com/playlist?list={listCode}"
            });
            playlistOutput = p.StandardOutput.ReadToEnd();
            return playlistOutput;
        }
        public static string getPlayListName(this string listCode)
        {
            string output = getPlayListInfo(listCode);
            return output.Split("[download] Finished downloading playlist: ")[1].Trim();
        }

    }
}
