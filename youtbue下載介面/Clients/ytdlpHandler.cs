using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面.Clients
{
    internal class ytdlpHandler
    {
        private string downloadUrl;
        public ytdlpHandler(string _url= "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe")
        {
            downloadUrl = _url; 
        }
        public async Task install()
        {
            using(var client = new HttpClient())
            {
                Stream stream = await client.GetStreamAsync(downloadUrl);
                using (var fileStream = File.Create(@".\yt-dlp.exe"))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }
}
