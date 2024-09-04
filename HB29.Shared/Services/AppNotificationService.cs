using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace hb29.Shared.Services
{
    public class AppNotificationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AppNotificationService> _logger;
        private readonly string _apiUrl;

        public AppNotificationService(IConfiguration config, ILogger<AppNotificationService> logger)
        {
            _config = config;
            _logger = logger;
            _apiUrl = _config.GetValue<string>("AppNotificationUrl");
        }

        public async Task<bool> SendMessage(string message, string auth)
        {
            try
            {
                if(auth == null)
                {
                    _logger.LogError($"occurred error in Send App Notification Request error: Token was not identified");
                    return false;
                }
                using (HttpClient client = new())
                {
                    HttpContent httpContent = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(new
                        {
                            Message = message
                        }),
                        Encoding.UTF8
                    );
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Replace("Bearer", "").Trim());

                    client.Timeout = TimeSpan.FromSeconds(60);

                    _logger.LogInformation($"Send App Notification Request to {_apiUrl}/notifications");
                    var result = await client.PostAsync($"{_apiUrl}/notifications", httpContent);

                    if (result.StatusCode == System.Net.HttpStatusCode.OK || result.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        _logger.LogInformation("App Notification sent.");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Error when send App Notification. {sendMessageError}", await result.Content.ReadAsStringAsync());
                        return false;
                    }
                } //using
            }
            catch (Exception ex)
            {
                _logger.LogError($"occurred error in Send App Notification Request error: {ex.Message}");
                return false;
            }

        }
    }
}