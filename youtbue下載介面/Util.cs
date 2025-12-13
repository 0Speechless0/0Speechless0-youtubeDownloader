namespace youtbue下載介面
{
    internal static class Util
    {
        public static string GetFileNameFromUrl(string rawUrl)
        {
            Uri uri = new Uri(rawUrl);
            var path = uri.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
            return GetFileNameFromRelativeUrl(path);

        }

        public static string GetFileNameFromRelativeUrl(string path)
        {
            var decodedPath = Uri.UnescapeDataString(path);

            return Path.GetFileName(decodedPath.TrimEnd('/'));
        }
    }

}