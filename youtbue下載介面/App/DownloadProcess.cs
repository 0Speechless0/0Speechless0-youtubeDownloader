using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using youtbue下載介面.Models;

namespace youtbue下載介面.App
{

    public class DownloadProcess
    {
        StringBuilder cmdOutput;

        CMDAppender _cMDAppender;
        DateTime downloadStart;
        internal DownloadProcess(CMDAppender cMDAppender)
        {
            cmdOutput = new StringBuilder();
            _cMDAppender = cMDAppender;
        }
        private void run(string arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = 
            _cMDAppender.os == "windows" ? "cmd.exe" : "python3";
            process.StartInfo.WorkingDirectory = @"./";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
                cmdOutput.Append(e.Data);
                Console.WriteLine(e.Data);

            });
            downloadStart = DateTime.Now;
            process.StartInfo.Arguments = arguments;
            Console.WriteLine($"開始執行: , {process.StartInfo.Arguments}");
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Dispose();
        }
        public void download()
        {
            _cMDAppender.AppendOutPutPath();
            _cMDAppender.AppendDowndUrl();
            run(_cMDAppender.GetCMD().ToString()) ;

        }
        public void downloadPlayList()
        {
            _cMDAppender.AppendPlayList();
            download();
        }

        public void update()
        {
            _cMDAppender.Append("--update-to master");
            run(_cMDAppender.GetCMD().ToString());
        }
    }
}