
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    public class Config
    {
        IConfigurationRoot config;
        public Config() {
             config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  
                .AddJsonFile("appsettings.json")
                .Build();
        }
        public string nextCloudHost {
            get {
                return config["nextCloudHost"].ToString();
            }
        }
    }
}
