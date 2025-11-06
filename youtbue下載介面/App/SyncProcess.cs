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
        DirectoryInfo[] directories;
        int dirCount;
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
        public void preparePush()
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
                return;
            }
            
            int page = 1;
            
            directories         = new DirectoryInfo(filePath).GetDirectories();
            dirCount            = directories.Count();
            while (true)
            {
                Dictionary<int, string> folderDic =
                    directories
                    .OrderByDescending(d => d.LastAccessTimeUtc)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select((value, index) => new { index, value.FullName })
                    .ToDictionary(x => x.index, x => x.FullName);

                foreach (KeyValuePair<int, string> keyValuePair in folderDic)
                {
                    Console.WriteLine($"{keyValuePair.Key } => {Path.GetFileName(keyValuePair.Value) }");
                }
                Console.WriteLine($"目前在第{page}頁 / 共 {getPageCount(dirCount)}頁, 輸入上傳資料夾 代號 或 頁碼+p : ");
                string typeRoute = Console.ReadLine() ?? "";

                if (Int32.TryParse(typeRoute, out int route))
                {
                    if (folderDic.TryGetValue(route, out string? folderFullName))
                        _dataObjectHandler.uploadFilesToCloud(folderFullName).Wait();
                    break;
                }
                else if (typeRoute.EndsWith("p"))
                {
                    if (Int32.TryParse(typeRoute.Remove(typeRoute.Length - 1), out int _page))
                    {
                        if (_page > 0 && _page < dirCount)
                            page = _page;
                        else
                            Console.WriteLine($"範圍:1p ~ {dirCount}p");
                    }
                    else
                        Console.WriteLine("請輸入合法頁碼格式(1p, 2p ... etc)");

                }
            }

        }
        public void pull()
        {
            _cloudHandler.uploadFiles(_filePath).Wait();
        }
    }
}