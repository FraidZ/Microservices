using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;      
        }

        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json"
            );
            
            var response = await httpClient.PostAsync($"{configuration["CommandService"]}", httpContent);
            Console.WriteLine($"--> Request to {configuration["CommandService"]}");

            Console.WriteLine(response.IsSuccessStatusCode
                ? "--> Sync Post to CommandService was ok!"
                : "--> Sync Post to CommandService was failed.");
        }
    }
}