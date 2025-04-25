using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            cmdOutput = new StringBuilder();
            _cMDAppender = cMDAppender;
            createProcess();
        }
        private void createProcess()
        {
            process = new Process();
            process.StartInfo.FileName = 
            _cMDAppender.os == "windows" ? "cmd.exe" : "python3";
            process.StartInfo.WorkingDirectory = @"./";
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
            process.Dispose();
            createProcess();
        }
        public void downloadPlayList()
        {
            _cMDAppender.AppendPlayList();
            download();
        }
    }
}