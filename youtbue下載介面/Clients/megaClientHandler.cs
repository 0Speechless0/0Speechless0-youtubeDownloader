using CG.Web.MegaApiClient;
using youtbue下載介面.App;
using youtbue下載介面.Interface;
using youtbue下載介面.Models;

namespace youtbue下載介面.Clients
{
    internal class megaClientHandler 
    {
        public bool isConnection {get;set;} = false;
        public bool hasRemoteUrl  {get;} = false;

        public string _targetFolderName;
        public string _tempDataFolderName;
        private MegaApiClient client;

        public megaClientHandler(
            string targetFolderName = "youtubeDownloader", 
            string tempDataFolderName = "data")
        {
            client = new MegaApiClient();
            _targetFolderName = targetFolderName;
            _tempDataFolderName = tempDataFolderName;
        }
        public async Task<DataObject> checkOrDownloadTempData()
        {
            // 取得所有節點
            IEnumerable<INode> nodes = client.GetNodes();

            // 找到目標資料夾
            INode? targetFolder = nodes.FirstOrDefault(n => n.Type == NodeType.Directory && n.Name == _tempDataFolderName);
            if (targetFolder is null)
            {
                Console.WriteLine($"找不到資料夾: {_targetFolderName}");
                client.Logout();
                return null;
            }

            //開始下載
            INode? fileNode = nodes.FirstOrDefault(n => n.Name == "tempData.bin" && n.Type == NodeType.File);

            if(fileNode is INode)
            {
                Console.WriteLine($"Downloading: {fileNode.Name}");

                var downloadStream = client.Download(fileNode);

                using (var fileStream = File.Create(@".\tempData.bin"))
                {
                    downloadStream.CopyTo(fileStream);
                }
            } 

            return Data.ReadFromBinaryFile<DataObject>(@".\tempData.bin");
        }

        public Task downloadByfilter(listObject listObject, string filter)
        {
            throw new NotImplementedException();
        }

        public bool login(DataObject dataObject)
        {
            var userinfo = dataObject.userinfo;
            client.Login(userinfo.account, userinfo.password);
            return true;
        }

        public Task updateTempData(string path)
        {
            throw new NotImplementedException();
        }

        public Task uploadFile(string? dir, string dirName, DateTime? beginTime)
        {
            throw new NotImplementedException();
        }
    }
}