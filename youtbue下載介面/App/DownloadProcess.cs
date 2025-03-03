using System.Diagnostics;

using System.Text;
using youtbue下載介面.Models;

namespace youtbue下載介面.App
{

    public class DownloadProcess
    {
        Process process ;
        StringBuilder cmdOutput;

        CMDAppender _cMDAppender;
        DateTime downloadStart;
        internal DownloadProcess(CMDAppender cMDAppender)
        {
            process = new Process();
            cmdOutput = new StringBuilder();
            _cMDAppender = cMDAppender;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
                cmdOutput.Append(e.Data);
                Console.WriteLine(e.Data);

            });
        }

        public void download()
        {
            _cMDAppender.AppendOutPutPath();
            _cMDAppender.AppendDowndUrl();
            downloadStart = DateTime.Now;
            process.StartInfo.Arguments = _cMDAppender.GetCMD().ToString();

            Console.WriteLine($"開始執行: , {process.StartInfo.Arguments}");
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
        public void downloadPlayList()
        {
            _cMDAppender.AppendPlayList();
            download();
        }
    }
}