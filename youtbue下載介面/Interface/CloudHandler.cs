using youtbue下載介面.Models;

namespace youtbue下載介面.Interface
{
    internal interface CloudHander
    {
        bool isConnection{get;set;}
        bool hasRemoteUrl { get; }

        Task<bool> login();

        Task<bool> pullRemoteData();
        Task<bool> pushLocalData();
        Task uploadFiles(string dir, DateTime? beginTime = null);
        Task downloadFiles(string dir, DateTime? beginTime = null);

    }
}
