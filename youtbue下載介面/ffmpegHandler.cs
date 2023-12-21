using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    internal class ffmpegHandler
    {
        private string downloadUrl;
        public ffmpegHandler(string _url= "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip")
        {
            downloadUrl = _url; 
        }
        public async Task install()
        {
            using(var client = new HttpClient())
            {
                Stream stream = await client.GetStreamAsync(downloadUrl);
                using (ZipArchive archive = new ZipArchive(stream))
                {
                    var targetFiles = archive.Entries.Where(e => Path.GetDirectoryName(e.FullName).EndsWith("-essentials_build\\bin") && e.Name != "" ).ToList();
                    foreach (ZipArchiveEntry entry in targetFiles)
                    {
                        if(!File.Exists(Path.Combine(@".\", entry.Name))) entry.ExtractToFile(Path.Combine(@".\", entry.Name));
                    }
                }
            }
        }
    }
}
