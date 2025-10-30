using youtbue下載介面.Clients;
using youtbue下載介面.Models;
namespace youtbue下載介面.App
{

    public class FeatureSwitcher{


        bool onlineMode = false;
        Dictionary<int, Feature> featureRouter ; 
        DownloadProcess _downloadProcess;
        SyncProcess _syncProcess;
        DataObjectHandler _dataObjectHandler;
        internal FeatureSwitcher(OS os, string filePath, DataObjectHandler  dataObjectHandler)
        {

            _downloadProcess    = new DownloadProcess(new CMDAppender(dataObjectHandler, os));
            _syncProcess        = new SyncProcess(dataObjectHandler, filePath);
            _dataObjectHandler = dataObjectHandler;
        }
        private Feature? GetCurrentFeature(int route)
        {

            return route switch
            {
                1 => new Feature
                {
                    name = "單一下載",
                    action = _downloadProcess.downloadOne,
                },
                2 => new Feature
                {
                    name = "曲單下載",
                    action = _downloadProcess.downloadPlayList
                },
                3 => new Feature
                {
                    name = "資料夾雲端上傳",
                    action = _syncProcess.push,
                    withCloud = true,
                },
                4 => new Feature
                {
                    name = "資料夾雲端下載",
                    action = _syncProcess.pull,
                    withCloud = true,
                },
                5 => new Feature
                {
                    name = "重製資料",
                    action = _dataObjectHandler.resetBin,
                    successMessage = "資料重製成功"
                },
                6 => new Feature
                {
                    name = "更新程式",
                    action = _downloadProcess.update
                },
                _ => null
            };
        }
        public void Run(bool onlineMode)
        {
            int route;
            while(true)
            {
                Console.WriteLine("功能選擇(請輸入數字1, 2 ,3 ... )");
                int i = 1;

                while (GetCurrentFeature(i) is Feature feature)
                {
                    if (!onlineMode && feature.withCloud)
                    {
                        i++;
                        continue;
                    }

                    Console.Write($"{i}=> {feature.name}, ");
                    i++;
   
                }

                if(Int32.TryParse( Console.ReadLine(), out route) )
                {
                    try{
                        GetCurrentFeature(route)?.Start(); 
                        _dataObjectHandler.writeToBin();
                    }
                    catch(Exception e)
                    {

                        
                    }
                    Console.WriteLine("繼續？(y/n)");
                    if( Console.ReadLine() == "n")
                    {
                        break;
                    }
                }

            }
        }
    }
}