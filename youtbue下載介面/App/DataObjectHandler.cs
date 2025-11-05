using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using youtbue下載介面.Clients;
using youtbue下載介面.Models;
using  youtbue下載介面.Interface;
using System.Reflection.Metadata;
namespace youtbue下載介面.App
{
    internal class DataObjectHandler
    {
        DataObject _dataObject { get; set; }
        CloudHander? _cloudHander = null;
 

        public bool cloudConnected {get;set;}
        public DataObjectHandler(Func<DataObject, CloudHander> createCloudHander)
        {
            string tempDataPath = Path.Combine(".", "tempData.bin");
            if (File.Exists(tempDataPath))
            {
                _dataObject = Data.ReadFromBinaryFile<DataObject>(tempDataPath) ?? new DataObject();
                // webDavHandler = new webDavHandler(DataObject, "youtubeDownload");

            }
            else
            {
                _dataObject = new DataObject();
            }
            // ListObjectArr = _dataObject.ListDic.ToArray();
            _cloudHander = createCloudHander.Invoke(_dataObject);
            willSetCloudUser();
        }

        public void willCloudSet()
        {
                        // checker: remote and user
            if (_dataObject.nextCloudUrl == null)
            {

                Console.WriteLine("nextCloud 資料上傳服務未設置，按enter 跳過，否則請先設置，輸入雲端位置(http(s)://...):");
                _dataObject.nextCloudUrl = Console.ReadLine();
            }
            else
            {
                 Console.WriteLine("無雲端連線建立，使用本地模式");
            }
        }
        private void willSetCloudUser()
        {
            if (_dataObject.nextCloudUrl == null)
                willCloudSet();
            if (_dataObject.nextCloudUrl == "")
                cloudConnected = false;
            Console.WriteLine("資料檢查中，請稍後...");
            try
            {
                do
                {

                    _cloudHander.login().GetAwaiter().GetResult();
                    if (_dataObject.userinfo.account != null && _cloudHander.isConnection)
                    {
                        _dataObject = _cloudHander.pullRemoteData(_dataObject).GetAwaiter().GetResult();
                         Console.WriteLine("雲端連線建立成功，使用雲端模式");
                        break;
                    }
                    else
                    {
                        if (_dataObject.userinfo.account == null)
                        {
                            Console.WriteLine("資料上傳使用者未設置，請先設置");
                        }
                        else
                        {
                            Console.WriteLine("輸入帳號認證失敗，請重新輸入");
                        }
                        Console.WriteLine("輸入帳號:");
                        _dataObject.userinfo.account = Console.ReadLine();
                        Console.WriteLine("請輸入密碼:");
                        _dataObject.userinfo.password = Console.ReadLine();
                        Console.WriteLine();
                        Console.WriteLine("請稍後...");
                    }

                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine("無雲端連線建立，使用本地模式");
                cloudConnected = false;
            }
            cloudConnected = true;
        }

        public async Task uploadFilesToCloud(string dir)
        {
            await _cloudHander.uploadFiles(dir);
        }

        public async Task saveFileName(string filePath)
        {
            string fullDirPath = Path.GetDirectoryName(filePath) ?? "";
            string fileName = Path.GetFileName(filePath);
            if (_dataObject.SongGroups.TryGetValue(fullDirPath, out List<string> fileArr))
            {
                _dataObject.SongGroups[fullDirPath]?.Add(fileName);
            } 
            else
            {
                _dataObject.SongGroups.Add(fullDirPath, new string[]{ Path.GetFileName(filePath)}.ToList() );                
            }

        }
        // public List<string> showListName()
        // {
        //     int i = 0;
        //     return _dataObject.ListDic.Select(row => $"({++i})[{row.Value.listName}]").ToList();
        // }

        
        // public listObject GetListObject(int index)
        // {
        //     return ListObjectArr[index - 1].Value;
        // }
        // public List<string> showListHistory(int index)
        // {
        //     int i = 0;
        //     return ListObjectArr[index -1].Value
        //         .HistoryDownloadList
        //         .Select(row => $"({++i})({row.Name})[{row.CreateTime}]").ToList();
        // }

        // public void updateDownloadIndex(string listCode, int i)
        // {
        //     _dataObject.ListDic[listCode].lastDownLoadIndex = i;
        // }

        public listObject setListObjectByCode(string listCode, string listName)
        {
            listObject targetList ;// checker : play list
            if ( ! _dataObject.ListDic.TryGetValue(listCode, out targetList))
            {
                targetList = new listObject
                {
                    listCode = listCode,
                    listName = listName

                };
                _dataObject.ListDic.Add(listCode, targetList);
                targetList.dirName = targetList.listName;

            }
            return targetList;

        }
        public void writeToBin(DataObject? dataObject = null)
        {
            Data.WriteToBinaryFile<DataObject>(@"tempData.bin",  dataObject ?? _dataObject);
        }
        public void resetBin()
        {
            _dataObject = new DataObject();
            Data.WriteToBinaryFile<DataObject>(@"tempData.bin",  _dataObject);
        }
    }
}
