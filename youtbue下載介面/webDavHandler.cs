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

namespace youtbue下載介面
{
    internal class webDavHandler
    {
        private bool auth = false;
        private WebDavClient webDavClient;
        PropfindResponse tempDataFileResult;
        public bool isConnection { get; set; }
        
        public webDavHandler()
        {
            isConnection = false;
        }
        public webDavHandler(DataObject data, string dir = "")
        {
            UserInfo userInfo = data.userinfo;

            var baseUrl = $"{data.nextCloudUrl}/remote.php/dav/files/{userInfo.account}/{dir}/";

            webDavClient = new WebDavClient(new WebDavClientParams
            {
                BaseAddress = new Uri(baseUrl),
                Credentials = new NetworkCredential(userInfo.account, userInfo.password)
            });
            tempDataFileResult = webDavClient.Propfind("data", new PropfindParameters {
                RequestType = PropfindRequestType.NamedProperties,
                Namespaces = new[] { 
                    new  NamespaceAttr("d", "DAV:"), 
                    new NamespaceAttr("oc", "http://owncloud.org/ns"),
                    new NamespaceAttr("nc", "http://nextcloud.org/ns")
                },
                CustomProperties = new[] { 
                    XName.Get("displayname", "DAV:") ,
                    XName.Get("getlastmodified", "DAV:") 
                }

            }).Result;
            
            if(tempDataFileResult.StatusCode == 404)
            {
                webDavClient.Mkcol($"../{dir}");
                webDavClient.Mkcol($"data");

            }
            if(tempDataFileResult.StatusCode != 401)
            {
                auth = true;
            }
            isConnection = true;



        }
        ~webDavHandler() { 
        
            webDavClient.Dispose();
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

        public async Task uploadFile(string? dir, string? dirName,DateTime downloadStart)
        {
            var files = Directory.GetFiles(dir)
                .Select(file => new FileInfo(file))
                .OrderBy(file => file.LastAccessTime)
                .Where(file => file.LastAccessTime > downloadStart);

            foreach (var file in files)
            {
                await webDavClient.PutFile($"{dirName}/{file.Name}", File.OpenRead(file.FullName));
            }

        }
        public async Task<DataObject> checkOrDownloadTempData()
        {
            if (tempDataFileResult.StatusCode == 404) return null;
            using (var response = webDavClient.GetRawFile("data/tempData.bin"))
            {
                using(var fileStream = File.Create(@".\tempData.bin"))
                {
                    response.Result.Stream.CopyTo(fileStream);
                }
            }
            return Data.ReadFromBinaryFile<DataObject>(".\\tempData.bin") ?? new DataObject();

        }

        public async Task<bool> checkAuth()
        {
            return auth;
        }


    }
}
