using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebDav;

namespace youtbue下載介面
{
    internal class webDavHandler
    {

        private WebDavClient webDavClient;
        public webDavHandler(string nextCloudHost, UserInfo userInfo)
        {
            var baseUrl = $"{nextCloudHost}/remote.php/dav/files/user/";
            webDavClient = new WebDavClient(new WebDavClientParams
            {
                BaseAddress = new Uri(baseUrl),
                Credentials = new NetworkCredential(userInfo.account, userInfo.password)
            });
            

        }
        ~webDavHandler() { 
        
            webDavClient.Dispose();
        }


    }
}
