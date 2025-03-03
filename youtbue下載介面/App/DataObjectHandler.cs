using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtbue下載介面.Clients;
using youtbue下載介面.Models;
namespace youtbue下載介面.App
{
    internal class DataObjectHandler
    {
        DataObject DataObject { get; set; }
        webDavHandler webDavHandler;
        public KeyValuePair<string, listObject>[] ListObjectArr { get; }

        public DataObjectHandler()
        {
            if (File.Exists(@".\tempData.bin"))
            {
                DataObject = Data.ReadFromBinaryFile<DataObject>(@".\tempData.bin") ?? new DataObject();
                webDavHandler = new webDavHandler(DataObject, "youtubeDownload");
            }
            else
            {
                DataObject = new DataObject();
            }
            ListObjectArr = DataObject.ListDic.ToArray();
        }
        public void willCloudSet()
        {
                        // checker: remote and user
            if (DataObject.nextCloudUrl == null)
            {

                Console.WriteLine("nextCloud 資料上傳服務未設置，按enter 跳過，否則請先設置，輸入雲端位置(http(s)://...):");
                DataObject.nextCloudUrl = Console.ReadLine();
            }
            else
            {
                 Console.WriteLine("無雲端連線建立，使用本地模式");
            }
        }
        public async Task<bool> willSetCloudUser()
        {
            if(DataObject.nextCloudUrl == null)
                willCloudSet();
            if(DataObject.nextCloudUrl == "")
                return false;
            Console.WriteLine("資料檢查中，請稍後...");
            try { 
                bool authCheck = false;
                while (!authCheck)
                {


                    if (webDavHandler != null && webDavHandler.checkAuth()) break;
                    else
                    {
                        if (DataObject.userinfo.account == null)
                        {
                            Console.WriteLine("nextCloud 資料上傳使用者未設置，請先設置");
                        }
                        else
                        {
                            Console.WriteLine("輸入帳號認證失敗，請重新輸入");
                        }
                        Console.WriteLine("輸入帳號:");
                        DataObject.userinfo.account = Console.ReadLine();
                        Console.WriteLine("請輸入密碼:");
                        DataObject.userinfo.password = Console.ReadLine();
                        Console.WriteLine();
                        Console.WriteLine("請稍後...");
                        webDavHandler = new webDavHandler(DataObject, "youtubeDownload");

                    }
                    if(webDavHandler == null)
                    {
                        webDavHandler = new webDavHandler(DataObject, "youtubeDownload");
                    }

                }
                if (webDavHandler.checkAuth())
                {
                    DataObject = await webDavHandler.checkOrDownloadTempData();
                }
            }
            catch (Exception e){
                Console.WriteLine("無雲端連線建立，使用本地模式");
                return false;
            }
            return true;
        }
        public List<string> showListName()
        {
            int i = 0;
            return DataObject.ListDic.Select(row => $"({++i})[{row.Value.listName}]").ToList();
        }

        public listObject GetListObject(int index)
        {
            return ListObjectArr[index - 1].Value;
        }
        public List<string> showListHistory(int index)
        {
            int i = 0;
            return ListObjectArr[index -1].Value
                .HistoryDownloadList
                .Select(row => $"({++i})({row.Name})[{row.CreateTime}]").ToList();
        }

        public void updateDownloadIndex(string listCode, int i)
        {
            DataObject.ListDic[listCode].lastDownLoadIndex = i;
        }

        public listObject setListObjectByCode(string listCode, string listName)
        {
            listObject targetList ;// checker : play list
            if ( ! DataObject.ListDic.TryGetValue(listCode, out targetList))
            {
                targetList = new listObject
                {
                    listCode = listCode,
                    listName = listName

                };
                DataObject.ListDic.Add(listCode, targetList);
                targetList.dirName = targetList.listName;

            }
            return targetList;

        }
    }
}
