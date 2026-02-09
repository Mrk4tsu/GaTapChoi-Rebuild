using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GaVL.BackgroundJobs;

public class BotNewsJobs
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    public BotNewsJobs(ILoggerFactory loggerFactory, HttpClient httpClient)
    {
        _logger = loggerFactory.CreateLogger<BotNewsJobs>();
        _httpClient = httpClient;
    }

    [Function("Bot")]
    public async Task Run([TimerTrigger("0 0 */1 * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        //string url = "http://localhost:5000/api/post/bot";
        string url = "https://api-gavl-ewhshvbfhwdccras.southeastasia-01.azurewebsites.net/api/post/bot";
        try
        {
            // Gọi method POST (hoặc GET tùy API của bạn)
            var response = await _httpClient.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Job triggered successfully!");
            }
            else
            {
                _logger.LogError($"Failed to trigger job. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
    }
}