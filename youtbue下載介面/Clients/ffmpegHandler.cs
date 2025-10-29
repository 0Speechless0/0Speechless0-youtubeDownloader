using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using youtbue下載介面.App;
using youtbue下載介面.Models;
namespace youtbue下載介面.Clients
{
    internal class ffmpegHandler
    {
        private string downloadUrl;
        Downloader downloader;
        public ffmpegHandler(string _url= "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip")
        {

            downloadUrl = _url; 
            downloader  = new Downloader(_url,  Path.Combine(Path.GetTempPath(), "ffmpeg-release-essentials.zip") );
            
        }
        public async Task installIfNotExist()
        {
            string filename = Path.Combine(".", "ffmpeg.exe");
            if (File.Exists(Path.Combine(".","ffmpeg.exe")) && File.Exists( Path.Combine(".", "ffplay.exe")) && File.Exists(Path.Combine(".", "ffprobe.exe")) )
            {
                return ;
            }
            Console.WriteLine("未發現ffmpeg，開始下載 .... ");
            Stream stream = await downloader.run();
            
            using (ZipArchive archive = new ZipArchive(stream))
            {
                var targetFiles = archive.Entries.Where(e => Path.GetDirectoryName(e.FullName).EndsWith("-essentials_build\\bin") && e.Name != "").ToList();
                foreach (ZipArchiveEntry entry in targetFiles)
                {
                    if (!File.Exists(Path.Combine(@".", entry.Name))) entry.ExtractToFile(Path.Combine(@".", entry.Name));
                }
            }
        }
    }
}
