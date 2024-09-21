using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Moyboy;
public class RuTubeRequest
{
    static async void DownloadSegmentAsync(string m3u8Url)
    {
        string baseAddress = m3u8Url.Substring(0, m3u8Url.LastIndexOf('/') + 1);
        string filePath = $"output-{DateTime.Now.Ticks}.mp4";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                Console.WriteLine("Загрузка фрагментов видео...");
                var m3u8Content = await client.GetStringAsync(m3u8Url);

                var segmentURLs = m3u8Content.Split('\n')
                                             .Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                                             .ToList();

                using (var fs = new FileStream(filePath, FileMode.CreateNew))
                {
                    foreach (var segmentUrl in segmentURLs)
                    {
                        var absoluteSegmentUrl = segmentUrl.StartsWith("http")
                            ? segmentUrl
                            : baseAddress + segmentUrl;

                        Console.WriteLine($"Скачивание сегмента: {absoluteSegmentUrl}");

                        var segmentData = await client.GetByteArrayAsync(absoluteSegmentUrl);
                        await fs.WriteAsync(segmentData, 0, segmentData.Length);
                    }
                }
                Console.WriteLine("Видео успешно загружено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
            }
        }
    }

    public static async Task GetURLs(string videoLink)
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        IWebDriver driver = new ChromeDriver(options);

        var xhrUrls = new List<string>();
        var handler = new NetworkRequestHandler();
        handler.RequestTransformer = (request) => { return request; };
        handler.RequestMatcher = httpRequest =>
        {
            if (httpRequest.Url.Contains(".m3u8"))
            {
                xhrUrls.Add(httpRequest.Url);
                return true;
            }
            return false;
        };

        INetwork networkInterceptor = driver.Manage().Network;
        networkInterceptor.AddRequestHandler(handler);

        await networkInterceptor.StartMonitoring();

        driver.Navigate().GoToUrl(videoLink);

        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        wait.Until(d => xhrUrls.Any());

        await networkInterceptor.StopMonitoring();

        if (xhrUrls.ToList() != null)
        {
            using (HttpClient client = new HttpClient())
            {
                Console.WriteLine("Загрузка разрешений...");
                var xhrContent = await client.GetStringAsync(xhrUrls.ToList()[0]);
                var segmentUrls = xhrContent.Split('\n')
                                        .Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                                        .ToList();
                Console.WriteLine("Доступные разрешения:");
                List<string> resolutions = new();
                int oi = 1;
                foreach (var segment in segmentUrls)
                {
                    await Console.Out.WriteLineAsync($"{oi++}. {segment.Substring(segment.LastIndexOf('=') + 1, segment.LastIndexOf('_') - segment.LastIndexOf('=') - 1)} " +
                        $"(сервер: {segment.Substring(segment.IndexOf(':') + 3, segment.IndexOf('.') - segment.IndexOf(':') - 3)})");
                }

                await Console.Out.WriteAsync("Вариант на скачивание: ");
                if (int.TryParse(Console.ReadLine(), out int downloadVar))
                {
                    var downloadString = segmentUrls[downloadVar - 1];
                    DownloadSegmentAsync(downloadString.Substring(0, downloadString.LastIndexOf('?')));
                }
            }
        }
        driver.Quit();
    }
}