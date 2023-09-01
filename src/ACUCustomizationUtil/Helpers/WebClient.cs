namespace ACUCustomizationUtils.Helpers;

public class WebClient : IDisposable
{
    private readonly Uri? _downloadUrl;
    private readonly string _destinationFilePath;

    private readonly HttpClient? _httpClient;
    private readonly IProgress<TaskProgressReport>? _progress;

    public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded,
        double? progressPercentage);

    public event ProgressChangedHandler? ProgressChanged;

    public WebClient(Uri downloadUrl, string destinationFilePath)
    {
        _downloadUrl = downloadUrl;
        _destinationFilePath = destinationFilePath;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromHours(1) };
        _progress = new Progress<TaskProgressReport>(OnProgressChangedHandler);
    }

    public async Task DownloadFileAsync()
    {
        var token = new CancellationToken();
        var response = await _httpClient!.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead, token);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"The request returned with HTTP status code {response.StatusCode}");
        }

        var total = response.Content.Headers.ContentLength ?? -1L;
        var fileInfo = new FileInfo(_destinationFilePath);
        if (fileInfo.Directory is { Exists: false })
        {
            fileInfo.Directory.Create();
        }

        await using var destination = File.OpenWrite(_destinationFilePath);
        await using var source = await response.Content.ReadAsStreamAsync(token);
        await CopyStreamWithProgressAsync(source, destination, total, token);
    }

    private async Task CopyStreamWithProgressAsync(Stream input, Stream output, long total, CancellationToken token)
    {
        const int ioBufferSize = 8 * 1024; // Optimal size depends on your scenario

        // Expected size of input stream may be known from an HTTP header when reading from HTTP. Other streams may have their
        // own protocol for pre-reporting expected size.

        var totalRead = 0L;
        var buffer = new byte[ioBufferSize];
        int read;
        var canReportProgress = total != -1 /*&& progress != null*/;

        while ((read = await input.ReadAsync(buffer, token)) > 0)
        {
            token.ThrowIfCancellationRequested();
            await output.WriteAsync(buffer.AsMemory(0, read), token);
            totalRead += read;
            if (canReportProgress)
            {
                _progress?.Report(new TaskProgressReport(total, totalRead));
            }
        }
    }

    private void OnProgressChangedHandler(TaskProgressReport tpr)
    {
        ProgressChanged?.Invoke(tpr.TotalFileSize, tpr.TotalBytesDownloaded, tpr.CurrentPercent);
    }

    private class TaskProgressReport
    {
        public TaskProgressReport(long totalFileSize, long totalBytesDownloaded)
        {
            TotalFileSize = totalFileSize;
            TotalBytesDownloaded = totalBytesDownloaded;
        }

        public long TotalFileSize { get; }
        public long TotalBytesDownloaded { get; }
        public int CurrentPercent => CalculatePercent();

        private int CalculatePercent()
        {
            var totalFileSize = Convert.ToDouble(TotalFileSize);
            var totalBytesDownloaded = Convert.ToDouble(TotalBytesDownloaded);
            var percent = Convert.ToInt16(Math.Round(totalBytesDownloaded / totalFileSize * 100, 0));

            return percent;
        }
    }

    public void Dispose() => GC.SuppressFinalize(this);
}