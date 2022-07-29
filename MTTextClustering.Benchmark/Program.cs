using MTTextClustering.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

class Program
{
    private static readonly HttpClient _client = new()
    {
        Timeout = Timeout.InfiniteTimeSpan
    };
    private static DataGeneratorModel _model = new(500, (ClusteringMethods)(-1), new Dictionary<string, object>
    {
        ["k"] = 2,
        ["threshold"] = 0.02,
        ["minimumPoints"] = 2,
        ["epsilon"] = 0.1,
        ["ts"] = 3
    });

    public static async Task Main(string baseUrl, string[] endpoints, ClusteringMethods method)
    {
        _model = _model with { Method = method };

        Console.WriteLine("Clustering method: {0}, Base URL: {1}", _model.Method.ToString(), baseUrl);

        var mainWatch = new Stopwatch();
        var requests = new List<Task>();
        var startTime = DateTime.UtcNow;

        mainWatch.Start();

        foreach (var endpoint in endpoints)
        {
            var json = JsonConvert.SerializeObject(_model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpointWatcher = new Stopwatch();
            var startEndpointTime = DateTime.UtcNow;
            endpointWatcher.Start();

            var request = _client.PostAsync(baseUrl + endpoint, content)
                .ContinueWith(t => t.Result.Content.ReadAsStringAsync())
                .ContinueWith(t =>
                {
                    endpointWatcher.Stop();
                    Console.WriteLine("{0} - {1} ms elapsed - +{2} ms from start time",
                        endpoint,
                        endpointWatcher.ElapsedMilliseconds,
                        startEndpointTime.Subtract(startTime).TotalMilliseconds);
                });

            requests.Add(request);
        }

        await Task.WhenAll(requests);
        mainWatch.Stop();

        Console.WriteLine("Main watcher: {0} ms elapsed", mainWatch.ElapsedMilliseconds);
        Console.ReadKey();
    }
}