using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private HttpClient _httpClient;
        private IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration) {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");

            Console.WriteLine($"Command Service endpoint {_configuration["CommandService"]}");

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                 Console.WriteLine("--> Sync POST TO CommandService was OK");
            }
            else
            {
                Console.WriteLine("--> Sync POST TO CommandService was not OK");
            }

        }
    }
}
