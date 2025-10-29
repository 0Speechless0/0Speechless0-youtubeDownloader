using System.ComponentModel;

namespace youtbue下載介面.App
{

    enum DownloadType
    {
        [Description("audio")]
        Audio = 0,

        [Description("video")]
        Video = 1,
    }
}