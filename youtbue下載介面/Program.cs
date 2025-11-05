// See https://aka.ms/new-console-template for more information


using PanoramicData.ConsoleExtensions;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using youtbue下載介面;
using youtbue下載介面.Models;
using youtbue下載介面.Clients;
using youtbue下載介面.App;
using System.Runtime.InteropServices;
using CG.Web.MegaApiClient;
using System.Security.Cryptography.X509Certificates;

Config config = new Config();

System.Net.ServicePointManager.ServerCertificateValidationCallback =
    (sender, cert, chain, sslPolicyErrors) => true;
    



// DataObjectHandler dataObjectHandler = new DataObjectHandler(() => new megaClientHandler("youtubeDownloader") );
OS os ;

ytdlpHandler?   ytdlpHandler    =   null;
ffmpegHandler   ffmpegHandler   =   new ffmpegHandler();
pythonInstaller pythonInstaller =   new pythonInstaller();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    ytdlpHandler = new ytdlpHandler(OS.Windows);
    await ffmpegHandler.installIfNotExist();
    os = OS.Windows;
}

else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    ytdlpHandler = new ytdlpHandler(OS.Linux);
    // 會安裝 ffmpeg
    await pythonInstaller.tryPipInstall();
    os = OS.Linux;
}
else
{
    Console.WriteLine("系統不支援");
    return; 
}
Console.Write("-------------歡迎使用youtube網址連結下載工具 ^__^------------ " +
    "\n\n注意:請確保歌單所有歌曲下載可行性\n\n\t\t\t\t\t\t\t\t\t\t\t作者:鄧臣宏(Alex) \n" +
    "------------------------------\n\n");
DataObjectHandler dataObjectHandler = new DataObjectHandler(
    (DataObject dataObject) => new webDavHandler(dataObject, "youtubeDownloader")
);

await ytdlpHandler.installIfNotExist();


FeatureSwitcher featureSwitcher = new FeatureSwitcher(os,   dataObjectHandler);
featureSwitcher.Run(dataObjectHandler.cloudConnected);
