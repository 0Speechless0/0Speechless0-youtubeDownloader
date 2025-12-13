using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using youtbue下載介面.App;

namespace youtbue下載介面.Clients
{
    internal class ytdlpHandler
    {
        Downloader downloader = null;
        string downloadUrl;
        string fileName  = "yt-dlp";
        public ytdlpHandler(OS os)
        {
            downloadUrl = os switch
            {
                OS.Windows => "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe",
                OS.Linux => "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp",
                _ => ""
            };
            fileName = Util.GetFileNameFromUrl(downloadUrl);
            downloader = new Downloader(downloadUrl, Path.Combine(".", fileName ));
        }
        public async Task installIfNotExist()
        {

            Uri uri = new Uri(downloadUrl);  // Parse the URL
            if(File.Exists(Path.Combine(".", fileName) ) )
            {
                return;
            }
            Console.WriteLine("未發現yt-dlp ，開始下載 .... ");

            await downloader.run();
            // using(var client = new HttpClient())
            // {
            //     Stream stream = await client.GetStreamAsync(downloadUrl);
            //     using (var fileStream = File.Create(Path.Combine(".", filename ) ))
            //     {
            //         stream.CopyTo(fileStream);
            //     }
            // }
        }



    }
}
