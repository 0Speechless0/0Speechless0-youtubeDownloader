using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using youtbue下載介面.Models;

namespace youtbue下載介面
{
    internal static class YtDlpUtil 
    {

        public static string? getListCode(this string url)
        {


            string[] urlArr = url.Split('/');
            string[] urlMainDiv = (urlArr.Length > 0 ? urlArr[urlArr.Length - 1] : "").Split('?');
            if(urlMainDiv.Length <2 )
            {
                Console.WriteLine("url 不帶參數，無法進行下載");
                return null;
            }
            string[] urlArg = urlMainDiv[1].Split('&');
            return urlArg.Length > 0 ? urlArg.FirstOrDefault(r => r.Contains("list=") )?.Split('=')[1] : "" ;
        }
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
                Arguments = $"yt-dlp --flat-playlist https://www.youtube.com/playlist?list={listCode}"
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
