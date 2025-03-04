using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task installIfNotExist(string downloadUrl)
        {

            Uri uri = new Uri(downloadUrl);  // Parse the URL
            string filename = System.IO.Path.GetFileName(uri.LocalPath); 
            if(File.Exists($@"./{filename}" ) )
            {
            
                return;
            }
            Console.WriteLine("未發現yt-dlp ，開始下載 .... ");
            using(var client = new HttpClient())
            {
                Stream stream = await client.GetStreamAsync(downloadUrl);
                using (var fileStream = File.Create($@"./{filename}"))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        private void runInstallation(string filename, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => Console.WriteLine("ERROR: " + args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
        private bool isInstalledOnPython(string argument)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{argument} --version",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        Console.WriteLine($"not found for '{argument}' . Error: {error}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            return true;
        }
        public async Task pipInstall()
        {
            if(!isInstalledOnPython(""))
            {
                runInstallation("/bin/bash", "-c \"sudo apt update && sudo apt install -y python3\"");
            }
            if(!isInstalledOnPython("-m pip"))
            {
                runInstallation("python3", "-m ensurepip --default-pip");

            }
            if(!isInstalledOnPython("-m yt_dlp"))
            {
                runInstallation("python3", "-m pip install --upgrade yt-dlp");

            }

        }

    }
}
