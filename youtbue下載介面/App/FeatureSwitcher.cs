using youtbue下載介面.Clients;
using youtbue下載介面.Models;
namespace youtbue下載介面.App
{

    public class FeatureSwitcher{


        bool onlineMode = false;
        Dictionary<int, Feature> featureRouter ; 
        DownloadProcess _downloadProcess;
        DataObjectHandler _dataObjectHandler;
        internal FeatureSwitcher(CMDAppender cMDAppender, DataObjectHandler  dataObjectHandler)
        {
            _downloadProcess = new DownloadProcess(cMDAppender);
            _dataObjectHandler = dataObjectHandler;
        }
        private Feature? GetCurrentFeature(int route)
        {
            
            return route switch 
            {
                1 => new Feature{
                   action = _downloadProcess.download
                },
                2 => new Feature{
                   action = _downloadProcess.downloadPlayList
                },
                4 => new Feature{
                   action = _dataObjectHandler.resetBin,
                   successMessage = "資料重製成功"
                },
                5 => new Feature{
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
                if (onlineMode)
                    Console.WriteLine("功能選擇(請輸入數字1, 2 ,3 ...) : 1.單一下載 2.歌單新曲下載 3.歌單舊曲查詢下載 4.重置程式資料 5. 更新程式");
                else
                    Console.WriteLine("功能選擇(請輸入數字1, 2 ...) : 1.單一下載 2.歌單新曲下載 4.重置程式資料 5. 更新程式");
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