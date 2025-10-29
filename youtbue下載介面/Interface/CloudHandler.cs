using youtbue下載介面.Models;

namespace youtbue下載介面.Interface
{
    internal interface CloudHander
    {
        bool isConnection{get;set;}
        bool hasRemoteUrl { get; }

        Task<bool> login();

        Task<DataObject> pullRemoteData(DataObject dataObject);
        Task uploadFiles(string dir, DateTime? beginTime = null);
        Task updateTempData(string path);

    }
}
