using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ddns_cpanel;

public class Worker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    private HttpClient client = new HttpClient();

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string uri = _configuration.GetConnectionString("cpanel");
        int ttl_ms = _configuration.GetSection("TimeToLive").GetValue<int>("hours") * 60 * 60 * 1000;


        while (!stoppingToken.IsCancellationRequested)
        {

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
                await UpdateDDNSAsync(uri);
            

            _logger.LogInformation("Waiting {time}ms before updating IP again", ttl_ms);
            await Task.Delay(ttl_ms, stoppingToken);
        }
    }

    private async Task UpdateDDNSAsync(string path)
    {
        HttpResponseMessage response = await client.GetAsync(path);

        string res = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Updated DDNS IP: {content}", res);
        }
        else
        {
            _logger.LogInformation("Failed to update DDNS IP: {content}", res);
        }
        
    }
}

