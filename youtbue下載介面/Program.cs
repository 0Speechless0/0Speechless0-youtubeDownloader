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

string uploadHost = new Config().nextCloudHost;


//if (!File.Exists(".\\yt-dlp.exe"))
//    System.Diagnostics.Process.Start("CMD.exe", "/C xcopy /Y /Q ..\\..\\..\\myBin\\ .\\ > nul");


Console.Write("-------------歡迎使用youtube網址連結下載工具 ^__^------------ " +
    "\n\n注意:請確保歌單所有歌曲下載可行性\n\n\t\t\t\t\t\t\t\t\t\t\t作者:鄧臣宏(Alex) \n" +
    "------------------------------\n\n");
DataObjectHandler dataObjectHandler = new DataObjectHandler(() => new webDavHandler("youtubeDownload") );
string os= "";

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    await new ytdlpHandler().installIfNotExist("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe");
    os ="windows";
}
else
{
    await new ytdlpHandler().installIfNotExist("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp");
    os="linux";
}

// await new ffmpegHandler().installIfNotExist();

var client = new MegaApiClient();
client.Login("kbb37038106@gmail.com", "a314622581246");
INode root = client.GetNodes().Single(n => n.Type == NodeType.Root);
bool cloudConnected = await dataObjectHandler.willSetCloudUser();
FeatureSwitcher featureSwitcher = new FeatureSwitcher(new CMDAppender(dataObjectHandler, os) );
featureSwitcher.Run(cloudConnected);
