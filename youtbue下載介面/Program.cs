// See https://aka.ms/new-console-template for more information


using PanoramicData.ConsoleExtensions;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using youtbue下載介面;

string uploadHost = new Config().nextCloudHost;
string strCmdText;
DateTime downloadStart;

//if (!File.Exists(".\\yt-dlp.exe"))
//    System.Diagnostics.Process.Start("CMD.exe", "/C xcopy /Y /Q ..\\..\\..\\myBin\\ .\\ > nul");

Console.Write("-------------歡迎使用youtube網址連結下載工具 ^__^------------ " +
    "\n\n注意:請確保歌單所有歌曲下載可行性\n\n\t\t\t\t\t\t\t\t\t\t\t作者:鄧臣宏(Alex) \n" +
    "------------------------------\n\n");
string url;
string[] urlArg;
string format;

string outputPath = null ;
int itemCount = 0;
listObject targetList = new listObject();

ffmpegHandler ffmpegHandler = new ffmpegHandler();
ytdlpHandler ytdlpHandler = new ytdlpHandler();


string userProfile =  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
DataObject dataObject = new DataObject();

if(!File.Exists(@".\yt-dlp.exe"))
{
    Console.WriteLine("未發現ytdlp組件，尋找下載...");
    await ytdlpHandler.install();
}

if (!File.Exists(@".\ffmpeg.exe") || !File.Exists(@".\ffplay.exe") || !File.Exists(@".\ffprobe.exe"))
{
    Console.WriteLine("未發現ffmpeg組件，尋找下載...");
    await ffmpegHandler.install();
}

try
{
    if (File.Exists(@".\tempData.bin"))
    {
        dataObject = Data.ReadFromBinaryFile<DataObject>(@".\tempData.bin") ?? new DataObject();
    }
    else
    {
        Data.WriteToBinaryFile(@".\tempData.bin", dataObject);
    }
}
catch(Exception e)
{
    File.Delete(@".\tempData.bin");
    Console.WriteLine("tempData.bin 錯誤，已將其刪除，請重新啟動");
    return;
}







if (dataObject.nextCloudUrl == null)
{

        Console.WriteLine("nextCloud 資料上傳服務未設置，按enter 跳過，否則請先設置，輸入雲端位置(http(s)://...):");
        dataObject.nextCloudUrl = Console.ReadLine();

}
Console.WriteLine("資料檢查中，請稍後...");
webDavHandler webDavHandler;
try { 

    webDavHandler = new webDavHandler(dataObject, "youtubeDownload");
    bool authCheck = false;
    while (!authCheck)
    {


        if (webDavHandler.checkAuth()) break;
        else
        {
            if (dataObject.userinfo.account == null)
            {
                Console.WriteLine("nextCloud 資料上傳使用者未設置，請先設置");
            }
            else
            {
                Console.WriteLine("輸入帳號認證失敗，請重新輸入");
            }
            Console.WriteLine("輸入帳號:");
            dataObject.userinfo.account = Console.ReadLine();
            Console.WriteLine("請輸入密碼:");
            dataObject.userinfo.password = ConsolePlus.ReadPassword();
            Console.WriteLine();
            Console.WriteLine("請稍後...");
            webDavHandler = new webDavHandler(dataObject, "youtubeDownload");



        }

    }
    if (webDavHandler.checkAuth())
    {
        dataObject = await webDavHandler.checkOrDownloadTempData();
    }
}
catch (Exception e){
    Console.WriteLine("無雲端連線建立，使用本地模式");
    webDavHandler = new webDavHandler();
}




while (true)
{
    StringBuilder cmd = new StringBuilder("/C yt-dlp");
    if (webDavHandler.checkAuth())
        Console.WriteLine("功能選擇(請輸入數字1, 2 ,3 ...) : 1.單一下載 2.歌單新曲下載 3.歌單舊曲查詢下載 4.重置程式資料 5. 更新程式");
    else
        Console.WriteLine("功能選擇(請輸入數字1, 2 ...) : 1.單一下載 2.歌單新曲下載 4.重置程式資料 5. 更新程式");
    if (!Int16.TryParse(Console.ReadLine(), out short route))
    {
        Console.WriteLine("請依上述格式輸入");
        continue;
    }
    if(route == 0)
    {
        Console.WriteLine("請輸入初始化的清單編碼 :");
        string code = Console.ReadLine();
        Test.init(dataObject, code);
        continue;
    }
    if (route == 3)
    {
        DataObjectHandler dataObjectHandler = new DataObjectHandler(dataObject);

        while (true)
        {
            dataObjectHandler.showListName().ForEach(text => Console.WriteLine(text));
            Console.WriteLine("請輸入清單代碼，輸入0結束:");
            listObject lookup;
            if (Int32.TryParse(Console.ReadLine(), out int index))
            {
                if (index == 0)
                {
                    break;
                }
                while (true)
                {
                    var historyList = dataObjectHandler.showListHistory(index);
                    lookup = dataObjectHandler.GetListObject(index);
                    historyList.Reverse();
                    historyList.ForEach(text => Console.WriteLine(text));
                    Console.WriteLine("請輸入代碼範圍，輸入 1~5 下載1,2,3,4,5紀錄的項目，輸入1,4,5 下載1 4 5 紀錄的項目:");
                    string filter = Console.ReadLine() ?? "";
                    try
                    {
                        Console.WriteLine("檔案下載中...");
                        await webDavHandler.downloadByfilter(lookup, filter);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("錯誤 :" + e.Message);
                        Console.WriteLine("請輸入數字!，如(1)[率性一下][2022-12-12]，要下載紀錄[率性一下]的項目，請輸入數字1");

                        continue;
                    }

                }

            }
            else
            {
                Console.WriteLine("請輸入數字!，如(1)[今天我不好]，要查看清單[今天我不好]紀錄，請輸入數字1");
                continue;
            }

        }
        continue;
    }
    if (route == 4)
    {
        File.Delete(".\\tempData.bin");
        Console.WriteLine("資料已重置...");
        break;
    }
    if (route == 5)
    {
        Console.WriteLine("更新中，請稍後......");
        var preProcess = System.Diagnostics.Process.Start(
            new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = "/C yt-dlp -U"
            });
        preProcess.WaitForExit();
        continue;
    }

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
        if (dataObject.ListDic.TryGetValue(listCode, out targetList))
        {
            //targetList.lastDownLoadIndex = 0;   
            Console.WriteLine($"此歌單已下載至第{targetList.lastDownLoadIndex}，下載後續新增{itemCount - targetList.lastDownLoadIndex}首...");
            if(itemCount - targetList.lastDownLoadIndex == 0)
            {
                Console.WriteLine("沒有新歌可以下載");
                continue;
            }
            cmd.Append($" -I {targetList.lastDownLoadIndex + 1}::1");


        }
        else {
            targetList = new listObject
            {
                listCode = listCode,
                listName = listCode.getPlayListName()

            };
            dataObject.ListDic.Add(listCode, targetList);
        }
        targetList.dirName = targetList.listName;
        //dataObject.ListDic.Add(listCode, new listObject
        //{


        //})

    }


    //if(targetList.dirName == null)
    //{
    //    Console.WriteLine("請輸入輸出資料夾名稱");
    //    targetList.dirName = Console.ReadLine();
    //}
   

    Console.WriteLine("請輸入下載格式(video/audio) :");
    string downloadType = Console.ReadLine().downLoadTypeCheck() ;
    if(downloadType.Length > 0)
    {
        if (downloadType.Equals("audio"))
        {
            Console.WriteLine("請輸入格式(best/aac/flac/mp3/m4a/opus/vorbis/wav): \n");
            format = Console.ReadLine();
            outputPath = Path.Combine(userProfile, $"Music\\{targetList.dirName}\\{targetList.dirName}-%(title)s.%(ext)s");
            cmd.Append($" -x --audio-format {format} -o \"{outputPath}\"");
        } 
        if(downloadType.Equals("video"))
        {
            outputPath = Path.Combine(userProfile, $"Videos\\{targetList.dirName}\\{targetList.dirName}-%(title)s.%(ext)s");
            cmd.Append($"  -o \"{outputPath}\"");
        }
        if(!Directory.Exists(Path.GetDirectoryName(outputPath)))
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
    }

    cmd.Append($" {url}");
    Console.WriteLine(cmd.ToString());
    var process = new System.Diagnostics.Process();
    StringBuilder cmdOutput = new StringBuilder();
    process.StartInfo.FileName = "cmd.exe";
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.Arguments = cmd.ToString();
    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
        cmdOutput.Append(e.Data);
        Console.WriteLine(e.Data);

    });
    downloadStart = DateTime.Now;
    process.Start();
    process.BeginOutputReadLine();

    process.WaitForExit();
    await Task.Delay(3000);
    //這裡不一定match到error
    if (Regex.Matches(cmdOutput.ToString(), "Error").Count == 0
        && Regex.Matches(cmdOutput.ToString(), "error").Count == 0
        && itemCount - targetList.lastDownLoadIndex > 0)
    {
        var historyDownload = new HistoryDownload();
        historyDownload.startIndex = targetList.lastDownLoadIndex;
        historyDownload.endIndex = targetList.lastDownLoadIndex + itemCount;
        targetList.lastDownLoadIndex = targetList.lastDownLoadIndex + itemCount;
        targetList.downloadCount++;


        if( webDavHandler.isConnection)
        {
            Console.WriteLine("上傳雲端中...");
            await webDavHandler.uploadFile(Path.GetDirectoryName(outputPath), targetList.listName, targetList.lastUploadTime);
            targetList.lastUploadTime = DateTime.Now;
            Console.WriteLine("請輸入這次下載系列的別名:");
            string partialName = Console.ReadLine();
            historyDownload.CreateTime = downloadStart;
            historyDownload.Name = partialName;
            targetList.HistoryDownloadList.Add(historyDownload);

            Data.WriteToBinaryFile(@".\tempData.bin", dataObject);
            await webDavHandler.updateTempData(@".\tempData.bin");
               
        }
        else
        {
            Data.WriteToBinaryFile(@".\tempData.bin", dataObject);
        }

    }

    
    Console.WriteLine("是否繼續?(y/n)");
    if (Console.ReadLine() != "y") break;

}