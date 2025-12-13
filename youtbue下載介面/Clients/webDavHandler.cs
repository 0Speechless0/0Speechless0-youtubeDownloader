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
        public bool hasRemoteUrl { get; } = true;
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



        private string rootDir;
        private bool cloudTempDataExists = false;

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
            tempDataFileResult = webDavClient.Propfind("../data/tempData.bin", propfindParamters).Result;



            if (tempDataFileResult.StatusCode != 401)
            {
                if (tempDataFileResult.StatusCode == 404)
                {
                    // webDavClient.Mkcol($"..{rootDir}");
                    webDavClient.Mkcol($"../data");

                }
                else
                {
                    cloudTempDataExists = true;
                }

                isConnection = true;
                return true;
            }
            isConnection = false;
            webDavClient.Dispose();
            return false;
        }

        public async Task uploadFiles(string fullDir, DateTime? beginTime)
        {
            beginTime = beginTime ?? DateTime.MinValue;

            string dir = Path.GetFileName(fullDir);
            webDavClient.Mkcol($"{dir}");
            HashSet<string> existsFile = (
                 _dataObject.SongGroups.TryGetValue(dir, out List<string>? fileArr) ? fileArr : new List<string>()
             ).ToHashSet<string>();

            var files = Directory.GetFiles(fullDir)
                .Select(file => new FileInfo(file))
                .OrderBy(file => file.CreationTime)
                .Where(file => file.CreationTime > beginTime);
            int i = 0;
            foreach (var file in files)
            {
                if (existsFile.Contains(file.Name))
                    continue;

                Console.WriteLine($"上傳 {file.Name} ({++i}/{files.Count()}) ...");

                var result = await webDavClient.PutFile($"{dir}/{file.Name}", File.OpenRead(file.FullName));
                if (result.IsSuccessful)
                {

                    if (_dataObject.SongGroups.ContainsKey(dir))
                    {
                        _dataObject.SongGroups[dir].Add(file.Name);
                    }
                    else
                    {
                        _dataObject.SongGroups.Add(dir, new string[] { file.Name }.ToList());
                    }
                }
            }
            await pushLocalData();

        }

        public async Task uploadFiles(string dir, string[] fileNames)
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
        public async Task<bool> pushLocalData()
        {
            string tempDataPath = Path.Combine(".", "tempData.bin");
            Data.WriteToBinaryFile<DataObject>(tempDataPath, _dataObject);
            var result = await webDavClient.PutFile($"../data/tempData.bin", File.OpenRead(tempDataPath));
            if (result.IsSuccessful)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> pullRemoteData()
        {
            string tempDataPath = Path.Combine(".", "tempData.bin");
            using (var response = await webDavClient.GetRawFile("../data/tempData.bin"))
            {
                // if (!cloudTempDataExists)
                // {
                //     Data.WriteToBinaryFile<DataObject>(tempDataPath, dataObject);
                //     await webDavClient.PutFile("data/tempData.bin", File.OpenRead(tempDataPath));                    
                //     return dataObject;
                // }
                if (cloudTempDataExists)
                {
                    using (var fileStream = File.Create(tempDataPath))
                    {
                        response.Stream.CopyTo(fileStream);
                    }
                    return true;
                }


            }
            _dataObject.SongGroups = new Dictionary<string, List<string>>();
            return false;

        }

        public bool checkAuth()
        {
            return auth;
        }

        public async Task downloadFiles(string fromRemoteDir, string toLocalDir, DateTime? beginTime = null)
        {
            var propfind = await webDavClient.Propfind(fromRemoteDir);

            if (!propfind.IsSuccessful)
            {
                Console.WriteLine($"PROPFIND 失敗: {propfind.StatusCode}");
                return;
            }

            var files = propfind.Resources
                                .Where(r => !r.IsCollection) // 只抓檔案
                                .ToList();

            foreach (var file in files)
            {
                string fileName = Util.GetFileNameFromRelativeUrl(file.Uri);
                string localPath = Path.Combine(toLocalDir, fileName);

                Console.WriteLine($"下載中: {fileName}");

                var fileResult = await webDavClient.GetRawFile(file.Uri);

                if (!fileResult.IsSuccessful)
                {
                    Console.WriteLine($"下載失敗: {fileName}");
                    continue;
                }

                using var fs = File.Create(localPath);
                await fileResult.Stream.CopyToAsync(fs);
            }
        }

        public async Task<string[]> GetDirs(DateTime? beginTime = null)
        {


            // 列出該資料夾下的所有項目
            var result = await webDavClient.Propfind("./");

            if (result.IsSuccessful)
            {
                // 過濾出資料夾
                var folders = result.Resources
                                    .Where(r => r.IsCollection) // 只選資料夾
                                    .Select(r => r.DisplayName);

                Console.WriteLine("資料夾清單:");
                foreach (var folder in folders)
                {
                    Console.WriteLine(folder);
                }

                return folders.ToArray();
            }
            else
            {
                Console.WriteLine("列出資料夾失敗: " + result.StatusCode);
                return new string[0];
            }
        }
    }
}
