// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using youtbue下載介面;
Console.WriteLine("Hello, World!");

string uploadHost = new Config().nextCloudHost;
string strCmdText;

if (!File.Exists(".\\yt-dlp.exe"))
    System.Diagnostics.Process.Start("CMD.exe", "/C xcopy /Y /Q ..\\..\\..\\myBin\\ .\\ > nul");

Console.Write("-------------歡迎使用youtube網址連結下載工具 ^__^------------ " +
    "\n\n注意:請確保歌單所有歌曲下載可行性\n\n\t\t\t\t\t\t\t\t\t\t\t作者:鄧臣宏(Alex) \n" +
    "------------------------------\n\n");
string url;
string[] urlArg;
string format;
string dirName;
string outputPath;
int itemCount = 0;
listObject targetList = null;
StringBuilder cmd = new StringBuilder("/C yt-dlp");

string userProfile =  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
Dictionary<string, listObject>
    ListDic;

if(File.Exists(".\\tempData.bin"))
{
    ListDic = Data.ReadFromBinaryFile<Dictionary<string, listObject>>(".\\tempData.bin") ?? new Dictionary<string, listObject>();
}
else
{
    ListDic = new Dictionary<string, listObject>();
}
    

while (true)
{
    Console.WriteLine("功能選擇(請輸入數字1 2 3) : 1.單一下載 2.歌單新曲下載 3.歌單舊曲查詢下載");
    Int16.TryParse(Console.ReadLine(), out short route);
    Console.WriteLine("請輸入來自youtube官方的下載連結:");
    url = Console.ReadLine();
    string[] urlArr = url.Split('/');
    string[] urlMainDiv = (urlArr.Length > 0 ? urlArr[urlArr.Length - 1] : "").Split('?');
    if(urlMainDiv.Length <2)
    {
        Console.WriteLine("url 不帶參數，無法進行下載");
        continue;
    }
    string urlEnd = urlMainDiv[0];
    urlArg = urlMainDiv[1].Split('&');
    if (route == 0) {
        Console.WriteLine("在功能選擇出現錯誤，請輸入數字");
        continue;
    }
    if(route == 1)
    {
        if (urlEnd.UrlTypeCheck() != route)
        {
            Console.WriteLine("要下載歌曲請確認url 路由為 /watch");
            continue;
        }
      
    }
    if (route == 2)
    {
        if (urlEnd.UrlTypeCheck() != route)
        {
            Console.WriteLine("要下載歌單請確認url 路由為 /playlist    ");
            continue;
        }
        cmd.Append(" --yes-playlist");
        
        string listCode = urlArg.Length > 0 ? urlArg[0].Split('=')[1] : null ;
        itemCount = listCode.getPlayListItemCount();
        if (ListDic.TryGetValue(listCode, out targetList))
        {

            Console.WriteLine($"此歌單已下載至第{targetList.lastDownLoadIndex}，繼續下載後續新增{itemCount - targetList.lastDownLoadIndex}...");
            cmd.Append($" -I {targetList.lastDownLoadIndex + 1}::1");


        }
        else {
            targetList = new listObject
            {
                listCode = listCode,
                listName = listCode.getPlayListName()

            };
            ListDic.Add(listCode, targetList);
        }
        //ListDic.Add(listCode, new listObject
        //{


        //})

    }

    Console.WriteLine("請輸入輸出資料夾名稱");
    dirName = Console.ReadLine();

    Console.WriteLine("請輸入下載格式(video/audio) :");
    string downloadType = Console.ReadLine().downLoadTypeCheck() ;
    if(downloadType.Length > 0)
    {
        if (downloadType.Equals("audio"))
        {
            Console.WriteLine("請輸入格式(best/aac/flac/mp3/m4a/opus/vorbis/wav): \n");
            format = Console.ReadLine();
            outputPath = Path.Combine(userProfile, $"Music\\{dirName}\\%(title)s.%(ext)s");
            cmd.Append($" -x --audio-format {format} -o {outputPath}");
        } 
        if(downloadType.Equals("video"))
        {
            outputPath = Path.Combine(userProfile, $"Videos\\{dirName}\\%(title)s.%(ext)s");
            cmd.Append($" -f best -o {outputPath}");
        }
    }
    cmd.Append($" {url}");
    Console.WriteLine(cmd.ToString());
    var process = System.Diagnostics.Process.Start( 
        new ProcessStartInfo
        {
            RedirectStandardOutput = true,
            FileName = "cmd.exe",
            Arguments = cmd.ToString()
        }    
    );
    process.WaitForExit();
    string pOutput = process.StandardOutput.ReadToEnd();
    if(targetList is listObject && Regex.Matches(pOutput, "ERROR").Count == 0)
    {
        targetList.startIndexHistory.Add(targetList.lastDownLoadIndex);
        targetList.lastDownLoadIndex = itemCount;
        targetList.downloadCount++;

        Data.WriteToBinaryFile(@"./tempData", ListDic);

    }

    Console.WriteLine("是否繼續?(y/n)");
    if (Console.ReadLine() != "y") break;  
}