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
        string _userProfile;
        string cmdOptions = "";
        StringBuilder _cmd;
        string? listCode;

        string _url= "";

        public OS os { get; set; }

        public CMDAppender(DataObjectHandler dataObjectHandler, OS os, string? userProfile = null)
        {

            _cmd = os switch
            {
                OS.Windows => new StringBuilder("/C yt-dlp"),
                OS.Linux => new StringBuilder("yt-dlp"),
                _ => new StringBuilder()
            };
            _userProfile = userProfile ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _dataObjectHandler = dataObjectHandler;

        }
        

        public string run()
        {
            string arguments = GetCMD().ToString();
            Process process = new Process();
            process.StartInfo.FileName = os switch
            {
                OS.Windows => "cmd.exe",
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
            return cmdOutput.ToString();
        }
        
        public bool hasList()
        {
            return listCode != null;
        }
        public void AppendDowndUrl(string url){
            _url = url;
            string[] urlArr = url.Split('/');
            string[] urlMainDiv = (urlArr.Length > 0 ? urlArr[urlArr.Length - 1] : "").Split('?');

            if(urlMainDiv.Length <2 )
            {
                Console.WriteLine("url 不帶參數，無法進行下載");
                return ;
            }
            string[] urlArg = urlMainDiv[1].Split('&');
            listCode = urlArg.Length > 0 ? urlArg.FirstOrDefault(r => r.Contains("list=") )?.Split('=')[1] : "" ;
            
        }
        public string AppendOutPutPath(DownloadType downloadType, string? format = null)
        {
            cmdOptions += downloadType switch
            {
                DownloadType.Audio => $" -x --audio-format {format}",
                _ => ""
            };
            string dirName = downloadType switch
            {
                DownloadType.Audio => "Music",
                DownloadType.Video => "Video",
                _ => ""
            };
            CMDCatcher cMDCatcher = new CMDCatcher(new CMDAppender(_dataObjectHandler, os));
            string? subDirName = listCode != null ? cMDCatcher.getPlayListName(listCode) : null;
            string outputPath = Path.Combine(
                _userProfile,
                dirName,
                $"{subDirName ?? "Default"}",
                $"%(title)s.%(ext)s"
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
        public void AppendPlayList(string[]? skipTitle = null)
        {
            skipTitle ??= Array.Empty<string>();
            cmdOptions += " --yes-playlist";
            string filterStr = string.Join("&", skipTitle.Select(e => $"title!='{e}'"));
            cmdOptions += filterStr.Length > 0 ? $" --match-filter \"{filterStr}\"" : "" ;
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