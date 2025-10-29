using System.Diagnostics;

namespace youtbue下載介面.Clients
{
    internal class pythonInstaller
    {
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
        public async Task tryPipInstall()
        {
            if(!isInstalledOnPython(""))
            {
                runInstallation("/bin/bash", "-c \"sudo apt update && sudo apt install -y python3\"");
            }
            if(!isInstalledOnPython("-m pip"))
            {
                runInstallation("python3", "-m ensurepip --default-pip");

            }
            // if(!isInstalledOnPython("-m yt_dlp"))
            // {
            //     runInstallation("python3", "-m pip install --upgrade yt-dlp");

            // }

        }
    }

}