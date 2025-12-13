using System.Text;
using youtbue下載介面.Models;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace youtbue下載介面.App
{

    internal class CMDAppender
    {
        DataObjectHandler _dataObjectHandler;
        StringBuilder cmdOutput;
        string cmdOptions = "";
        StringBuilder _cmd;
        string? listCode;

        string _url= "";
        string musicFolder;
        string videoFolder;
        public OS _os { get; set; }

        public CMDAppender(DataObjectHandler dataObjectHandler, OS os)
        {

            _os = os;
            _cmd = new StringBuilder();
            _dataObjectHandler = dataObjectHandler;
            cmdOutput = new StringBuilder();

            musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            videoFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        }
        

        public string run()
        {
            string arguments = GetCMD().ToString();
            Process process = new Process();
            process.StartInfo.FileName = _os switch
            {
                OS.Windows => "yt-dlp.exe",
                OS.Linux => "python3",
                _ => ""
            };

            process.StartInfo.WorkingDirectory = @"./";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                cmdOutput.Append(e.Data);
                Console.WriteLine(e.Data);

            });
            process.StartInfo.Arguments = arguments;
            Console.WriteLine($"開始執行: , {process.StartInfo.Arguments}");
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Dispose();

            cmdOptions = "";
            _cmd.Clear();
            return cmdOutput.ToString();
        }
        
        public bool hasList()
        {
            return listCode != null;
        }
        public void AppendDowndUrl(string url){

            string[] urlArr      = url.Split('/');
            string   url_path    = url.Split('?')[0];
            string[] urlMainDiv = (urlArr.Length > 0 ? urlArr[urlArr.Length - 1] : "").Split('?');
            string[] urlArg = urlMainDiv[1].Split('&');
            string? videoArg = urlArg.FirstOrDefault(e => e.Contains("v="));
            listCode = urlArg.Length > 0 ? urlArg.FirstOrDefault(r => r.Contains("list=") )?.Split('=')[1] : "" ;
            _url = $"{url_path}?{videoArg}";
            if(url_path.Split("/").LastOrDefault() == "playlist" )
            {
                _url += $"&list={listCode}";
            }
        }
        
        public string AppendOutPutPath(DownloadType downloadType)
        {
            cmdOptions += downloadType switch
            {
                DownloadType.Audio => $" -x --embed-thumbnail --add-metadata --audio-format best",
                DownloadType.Video => " -f bestvideo+bestaudio --merge-output-format mp4"
            };
            string folder = downloadType switch
            {
                DownloadType.Audio => musicFolder,
                DownloadType.Video => videoFolder,
                _ => ""
            };
            CMDCatcher cMDCatcher = new CMDCatcher(new CMDAppender(_dataObjectHandler, _os));
            string? subDirName = cMDCatcher.getPlayListName(listCode);
            subDirName = string.IsNullOrEmpty(subDirName) ? "Default" : subDirName;
            string outputPath = Path.Combine(
                folder,
                $"{subDirName}",
                $"%(title)s-%(id)s.%(ext)s"
            ); 
            cmdOptions += $" -o \"{outputPath}\"";
            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            return outputPath;
    }
        // private void AppendPlayListRange()
        // {
        //                 //targetList.lastDownLoadIndex = 0;
        //     int itemCount = listCode.getPlayListItemCount();   
        //     Console.WriteLine($"此歌單已下載至第{appendingListObject.lastDownLoadIndex}，下載後續新增{itemCount - appendingListObject.lastDownLoadIndex}首...");
        //     if(itemCount - appendingListObject.lastDownLoadIndex == 0)
        //     {
        //         Console.WriteLine("沒有新歌可以下載");
        //         return;
        //     }
        //     cmdOptions += ! cmdOptions.Contains(" -I ") ? $" -I {appendingListObject.lastDownLoadIndex + 1}::1" : "";
        // }
        public void AppendPlayList(string[]? skipID = null)
        {
            skipID ??= Array.Empty<string>();
            cmdOptions += " --yes-playlist";
            string filterStr = string.Join("&", skipID.Select(e => $"id!='{e}'"));
            cmdOptions += filterStr.Length > 0 ? $" --match-filter \"{filterStr}\"" : "" ;
        }

        public void Append(string str)
        {
            cmdOptions += $" {str}"; 
        }
        
        public StringBuilder GetCMD()
        {
            string start_arg = _os switch
            {
                OS.Windows => "",
                OS.Linux => "yt-dlp",
                _ => ""
            };
            _cmd.Append($"{start_arg}");
            _cmd.Append($" {cmdOptions}");
            _cmd.Append($" {_url}");
            return _cmd;
        }
    }
}