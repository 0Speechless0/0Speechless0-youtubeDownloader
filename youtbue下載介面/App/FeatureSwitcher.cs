using youtbue下載介面.Clients;
namespace youtbue下載介面.App
{

    public class FeatureSwitcher{

        webDavHandler webDavHandler;

        bool onlineMode = false;
        Dictionary<int, Feature> featureRouter ; 
        DownloadProcess downloadProcess;
        internal FeatureSwitcher(webDavHandler webDavHandler, CMDAppender cMDAppender)
        {
            onlineMode = webDavHandler.checkAuth();
            featureRouter = new Dictionary<int, Feature>()
            {
                {0, new Feature() }
            };
            downloadProcess = new DownloadProcess(cMDAppender);
        }
        private Feature? GetCurrentFeature(int route)
        {
            
            return route switch 
            {
                1 => new Feature{
                   action = downloadProcess.download
                },
                2 => new Feature{
                   action = downloadProcess.downloadPlayList
                },
                _ => null
            };
        }
        public void Run()
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
                    }
                    catch(Exception e)
                    {


                    }
                    Console.WriteLine("請問是否繼續？(y/n)");
                    if( Console.ReadLine() == "y")
                    {
                        break;
                    }
                }

            }
        }
    }
}