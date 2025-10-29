using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebDav;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using youtbue下載介面.Models;
using youtbue下載介面.App;
using youtbue下載介面.Interface;
using System.Net.Http.Headers;
// 全域忽略 SSL 驗證

namespace youtbue下載介面.Clients
{
    internal class webDavHandler : CloudHander
    {
        private bool auth = false;
        public bool hasRemoteUrl  {get;} = true;
        private WebDavClient webDavClient;

        DataObject _dataObject;
        PropfindResponse tempDataFileResult;
        PropfindParameters propfindParamters = new PropfindParameters
        {
            RequestType = PropfindRequestType.NamedProperties,
            Namespaces = new[] {
                    new  NamespaceAttr("d", "DAV:"),
                    new NamespaceAttr("oc", "http://owncloud.org/ns"),
                    new NamespaceAttr("nc", "http://nextcloud.org/ns")
                },
            CustomProperties = new[] {
                    XName.Get("displayname", "DAV:") ,
                    XName.Get("getlastmodified", "DAV:") ,
                    XName.Get("getcontenttype", "DAV:")
                }
        };
        private string videoDir;
        private string audioDir;

        private string rootDir;
        public bool isConnection { get; set; } = false;


        //private bool startUpload;
        //public Queue<string> uploadFileQueue = new Queue<string>();
        public webDavHandler(
        )
        {
            isConnection = false;

        }
        public webDavHandler(DataObject dataObject, string dir = "")
        {
            rootDir = $"/{dir}";
            var userProfileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            audioDir = Path.Combine(userProfileDir, "Music");
            videoDir = Path.Combine(userProfileDir, "Videos");
            _dataObject = dataObject;
        }

        async public Task<bool> login()
        {
            var u = _dataObject?.userinfo;
            var accountDir = u.account == null ? "" : $"/{u.account}";
            var baseUrl = $"{_dataObject?.nextCloudUrl}/remote.php/dav/files{accountDir}{rootDir}/";
            // 1) 建立 HttpClientHandler，跳過憑證驗證（測試用）
            var handler = new HttpClientHandler
            {
                // 這裡直接回傳 true，略過憑證驗證（含名稱與鏈）
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,

                // 可把 Credentials 放在 handler 上（若伺服器使用 Windows auth / NTLM）
                Credentials = new NetworkCredential(u.account, u.password)
            };

            // 2) 建立 HttpClient，並設定 BaseAddress
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            // 3) 如果 WebDAV 伺服器用 Basic Auth，預先送出 Authorization header（可避免 401 再挑戰）
            var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{u.account}:{u.password}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicToken);

            webDavClient = new WebDavClient(httpClient);
            tempDataFileResult = webDavClient.Propfind("data", propfindParamters).Result;
            
            if(tempDataFileResult.StatusCode == 404)
            {
                webDavClient.Mkcol($"..{rootDir}");
                webDavClient.Mkcol($"data");

            }
            if(tempDataFileResult.StatusCode != 401)
            {
                isConnection = true;
                return true;
            }
            isConnection = false;
            webDavClient.Dispose();
            return false;
        }
        public  async Task<bool>  updateOrCreateTempData()
        {

            var tempDataFile = tempDataFileResult.Resources.Where(file => file.DisplayName == "tempData.bin").FirstOrDefault();
            var localTempData = Directory.GetFiles(@".\").Select(file => new FileInfo(file))
                .Where(file => file.Name == "tempData.bin")
                .First();

            if (tempDataFile == null)
            {
                var uploadResult = await webDavClient.PutFile("data/tempData.bin", File.OpenRead(@".\tempData.bin"));
                localTempData.LastWriteTime = DateTime.Now;
                return false;
            }
            else if(localTempData.LastWriteTime < tempDataFile.LastModifiedDate)
            {
                return true;
            }
            return false;
        }
        public async Task updateTempData(string path)
        {
            var uploadResult = await webDavClient.PutFile("data/tempData.bin", File.OpenRead(path));
        }


        //public void insertNewestFileToQueue(string? dir)
        //{
        //    var file = Directory.GetFiles(dir)
        //        .Select(file => new FileInfo(file))
        //        .Where(file => !file.Name.Contains(".webm"))
        //        .OrderByDescending(file => file.CreationTime)
        //        .FirstOrDefault();
        //    uploadFileQueue.Enqueue(file.Name);
        //}
        //public void endUpload()
        //{
        //    this.startUpload = false;
        //}
        //public bool isUploading()
        //{
        //    return startUpload || uploadFileQueue.Count > 0;
        //}
        //public async Task uploadNewestFileInQueue(string? dir, string dirName)
        //{
        //    startUpload = true;
        //    do
        //    {
        //        if(uploadFileQueue.Count > 0)
        //        {
        //            var fileName = uploadFileQueue.Dequeue();
        //            Console.WriteLine($"上傳 {fileName} ...");
        //            var fullName = Path.Combine(dir, fileName);
        //            var result = await webDavClient.PutFile($"{dirName}/{fileName}", File.OpenRead(fullName));
        //        }

        //    } while (startUpload || uploadFileQueue.Count > 0);

        //    //var file = Directory.GetFiles(dir)
        //    //    .Select(file => new FileInfo(file))
        //    //    .Where(file => !file.Name.Contains(".webm"))
        //    //    .OrderByDescending(file => file.CreationTime)
        //    //    .FirstOrDefault();
        //    //webDavClient.Mkcol($"{dirName}");
        //    //if (file != null)
        //    //{
        //    //    Console.WriteLine($"上傳 {file.Name} ...");

        //    //    var result = await webDavClient.PutFile($"{dirName}/{file.Name}", File.OpenRead(file.FullName));
        //    //}    
        //}
        public async Task uploadFiles(string dir, DateTime? beginTime)
        {
            beginTime = beginTime ?? DateTime.MinValue;
            var files = Directory.GetFiles(dir)
                .Select(file => new FileInfo(file))
                .OrderBy(file => file.CreationTime)
                .Where(file => file.CreationTime > beginTime);
            string dirName = Path.GetFileName(dir);
            webDavClient.Mkcol($"{dirName}");
            int i = 0;
            foreach (var file in files)
            {
                Console.WriteLine($"上傳 {file.Name} ({++i}/{files.Count()}) ...");

                var result = await webDavClient.PutFile($"{dirName}/{file.Name}", File.OpenRead(file.FullName));
            }

        }
        public async Task uploadFiles(string dir,  string[] fileNames)
        {
            var files = Directory.GetFiles(dir)
                .Select(file => new FileInfo(file))
                .OrderBy(file => file.CreationTime);
            string dirName = Path.GetFileName(dir);
            webDavClient.Mkcol($"{dirName}");
            int i = 0;
            foreach (var name in fileNames)
            {
                Console.WriteLine($"上傳 {name} ({++i}/{files.Count()}) ...");

                var result = await webDavClient.PutFile($"{dirName}/{name}", File.OpenRead(Path.Combine(dir, name)));
            }

        }
        public async Task<DataObject> pullRemoteData(DataObject dataObject)
        {
            string tempDataPath = Path.Combine(".", "tempData.bin");
            using (var response = await webDavClient.GetRawFile("data/tempData.bin"))
            {
                if (response.StatusCode == 404)
                {
                    Data.WriteToBinaryFile<DataObject>(tempDataPath, dataObject);
                    await webDavClient.PutFile("data/tempData.bin", File.OpenRead(tempDataPath));                    
                    return dataObject;
                }
                else
                {
                    using (var fileStream = File.Create(tempDataPath))
                    {
                        response.Stream.CopyTo(fileStream);
                    }
                }


            }
            return Data.ReadFromBinaryFile<DataObject>(tempDataPath) ;

        }

        public bool checkAuth()
        {
            return auth;
        }



    }
}
