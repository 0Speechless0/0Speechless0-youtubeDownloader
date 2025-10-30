using System.Diagnostics;
using System.Text.RegularExpressions;
using youtbue下載介面.Models;

namespace youtbue下載介面.App
{

    internal class CMDCatcher{
        ProcessStartInfo processInfo;
        CMDAppender _cMDAppender;

        public CMDCatcher(CMDAppender cMDAppender)
        {
            _cMDAppender = cMDAppender;
        }
        public string[] getSongsInPlayList(string listCode)
        {
            //if (playlistOutput != null) return playlistOutput;
            _cMDAppender.Append($" --flat-playlist \"%(title)s\"   https://www.youtube.com/playlist?list={listCode}");
            return _cMDAppender.run().Split("\n");

        }
        public int getPlayListItemCount(string listCode)
        {
            string[] songsList = getSongsInPlayList(listCode);
            return songsList.Length;
        }
        
        public string getPlayListName(string listCode)
        {
            _cMDAppender.Append($" --quiet --print \"%(playlist_title)s\"  --flat-playlist --playlist-end 1  https://www.youtube.com/playlist?list={listCode}"); 
            return _cMDAppender.run().Split("\n").Last();
        }
        public listObject[] getPlayListObjects(string userName)
        {
            _cMDAppender.Append($" --flat-playlist --print \"%(title)s|&|%(id)s\" https://www.youtube.com/@{userName}/playlists");
            string[] lists =  _cMDAppender.run().Split("\n");
            return lists
            .Select(e => e.Split("|&|") )
            .Select( e =>
            new listObject{
                dirName = e[0],
                listCode = e[1]
            }).ToArray<listObject>();
        }
    }
}