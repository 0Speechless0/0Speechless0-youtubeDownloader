using System.Security.AccessControl;
using System.Threading.Tasks;
using youtbue下載介面.Interface;
using youtbue下載介面.Models;
namespace youtbue下載介面.App
{

    internal class SyncProcess
    {
        string _filePath;

        DataObjectHandler _dataObjectHandler;
        int pageSize = 10;
        CloudHander _cloudHandler;

        public SyncProcess(DataObjectHandler dataObjectHandler)
        {
            _dataObjectHandler  = dataObjectHandler;
        }

        private int getPageCount(int count)
        {
            if (count % pageSize == 0)
                return count / pageSize;
            else
            {
                return count / pageSize + 1;
            }
        }
        
        private string getTypePath()
        {
            Console.WriteLine("請選擇要上傳的類別(0 => 音樂, 1 => 影片): ");
            string n = Console.ReadLine();

            string filePath = n switch 
            {
                "0" => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                "1" => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                _ => ""
            };

            if (filePath == "")
            {
                Console.WriteLine("無效的類別");
            }
            return filePath;
        }
        private int DirSelector(Dictionary<int, string> folderDic, ref int page , int dirCount)
        {
            
            foreach (KeyValuePair<int, string> keyValuePair in folderDic)
            {
                Console.WriteLine($"{keyValuePair.Key } => {Path.GetFileName(keyValuePair.Value) }");
            }
            Console.WriteLine($"目前在第{page}頁 / 共 {getPageCount(dirCount)}頁, 輸入上傳資料夾 代號 或 頁碼+p, 輸入0p結束 : ");
            string typeRoute = Console.ReadLine() ?? "";

            if (Int32.TryParse(typeRoute, out int route))
            {

                return route;
            }
            else if (typeRoute.EndsWith("p"))
            {
                if (Int32.TryParse(typeRoute.Remove(typeRoute.Length - 1), out int _page))
                {
                    page = _page;
                    if (_page < 0 && _page >= dirCount)
                        Console.WriteLine($"範圍:1p ~ {dirCount}p");
                }
                else
                    Console.WriteLine("請輸入合法頁碼格式(1p, 2p ... etc)");

            }
            return -1;
        }
        public void preparePush()
        {
            string localTypePath = getTypePath();
            
            int page = 1;
            
            DirectoryInfo[] directories         = new DirectoryInfo(localTypePath).GetDirectories();
            int dirCount                        = directories.Count();
            while (true)
            {
                Dictionary<int, string> folderDic =
                    directories
                    .OrderByDescending(d => d.LastAccessTimeUtc)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select((value, index) => new { index, value.FullName })
                    .ToDictionary(x => x.index, x => x.FullName);

                int route = DirSelector(folderDic, ref page, dirCount);
                if (folderDic.TryGetValue(route, out string? folderFullName))
                {
                    _dataObjectHandler.uploadFilesToCloud(folderFullName).Wait();
                    break;
                }
                if(page == 0)
                    break;

            }

        }
        public void preparePull()
        {
            
            string localTypePath = getTypePath();
            string[] dirs   = _dataObjectHandler.GetCloudDirs().GetAwaiter().GetResult();
            int page        = 1;
            int dirCount    = dirs.Count();
            while (true)
            {
                Dictionary<int, string> folderDic = dirs                    
                    .Select((value, index) => new { index, value })
                    .ToDictionary(x => x.index, x => x.value);
                int route = DirSelector(folderDic, ref page, dirCount);
                if (folderDic.TryGetValue(route, out string? folderFullName))
                {
                    _dataObjectHandler.downloadFilesFrom(folderFullName, localTypePath).Wait();
                    break;
                }
                if(page == 0)
                    break;

            }

        }
    }
}