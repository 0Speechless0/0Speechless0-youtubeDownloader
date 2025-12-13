
namespace youtbue下載介面.App
{

    internal class Downloader
    {
        string _downloadUrl;
        string _outputPath;
        public Downloader(string downloadUrl, string outputPath)
        {
            _downloadUrl = downloadUrl;
            _outputPath = outputPath;
        }
        async internal Task<FileStream> run()
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            };

            using var response = await httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1;

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(_outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int read;

            DateTime lastUpdate = DateTime.Now;

            while ((read = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;

                // 每 100ms 更新一次進度條，避免太頻繁閃爍
                if ((DateTime.Now - lastUpdate).TotalMilliseconds > 1000)
                {
                    if (canReportProgress)
                    {
                        Console.WriteLine(@$"\下載進度：{(float)totalRead / totalBytes * 100:F1} %");
                    }
                    lastUpdate = DateTime.Now;
                }
            }

            Console.WriteLine("\n下載完成。");
            return fileStream;
        }
    }

}
