using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace youtbue下載介面
{
    internal static class Standard
    {
        public static int UrlTypeCheck(this string urlEnd) => urlEnd switch
        {
            "watch" => 1,
            "playlist" => 2,
            _ => 0
        };
        public static string downLoadTypeCheck(this string downloadType) => downloadType switch
        {
            "video" => "video",
            "audio" => "audio",
            _ => ""
        };




    }
}
