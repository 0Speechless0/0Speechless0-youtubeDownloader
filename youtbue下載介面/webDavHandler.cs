using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    internal class webDavHandler
    {
        private string baseUrl;
        public webDavHandler()
        {
            var config = new Config();
            baseUrl = $"http://{config.nextCloudHost}/remote.php/dav/files/user/";
        }
    }
}
