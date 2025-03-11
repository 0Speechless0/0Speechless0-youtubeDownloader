using youtbue下載介面.Models;

namespace youtbue下載介面.Interface
{
    internal interface CloudHander
    {
        bool isConnection{get;set;}
        Task<DataObject> checkOrDownloadTempData();
        bool login(DataObject dataObject);
        Task uploadFile(string? dir, string dirName, DateTime? beginTime);
        Task updateTempData(string path);

        Task downloadByfilter(listObject listObject, string filter);
    }
}
