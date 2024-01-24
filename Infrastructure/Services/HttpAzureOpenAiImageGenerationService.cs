using ChGPTcmd.Application.Services;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using ChGPTcmd.Infrastructure.DTOs;
using Microsoft.Extensions.Options;
using ChGPTcmd.Infrastructure.Configuration.Options;

namespace ChGPTcmd.Infrastructure.Services
{
    public class HttpAzureOpenAiImageGenerationService : ImageGenerationService
    {
        private ILogger<HttpOpenAiImageGenerationService> logger;
        private HttpClient httpClient;
        private AzureOpenAiOptions openAiOptions;

        public HttpAzureOpenAiImageGenerationService(IOptions<AzureOpenAiOptions> openAiOptions, ILogger<HttpOpenAiImageGenerationService> logger)
        {
            this.openAiOptions = openAiOptions.Value;
            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("api-key", this.openAiOptions.ApiKey);
            httpClient.DefaultRequestHeaders.Add("x-ms-useragent", "AzureOpenAIImageConsole/0.0.1");
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {this.openAiOptions.ApiKey}");

            this.logger = logger;
        }

        public async Task Post(string prompt)
        {
            HttpRequestMessage requestMessage = CreateHttpRequestMessage(prompt, 1, 1024);
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
            
            string? id = await GetGenerationIdAsync(response);

            Console.WriteLine();
            Console.WriteLine($"Your ID: {id}");
            Console.WriteLine();
            Console.WriteLine("Image(s) will be generated...");
            var checkEndpoint = GetCheckEndpoint(id);
            
            bool isFinished = false;
            do
            {
                await Task.Delay(2000);
                HttpResponseMessage checkResponse = await httpClient.GetAsync(checkEndpoint);
                (bool success, ImageResponseItemDto? imageCreationResponse) = await GetImageCreationResponseAsync(checkResponse);
                isFinished = success;
                if (success && imageCreationResponse is not null)
                {
                    string urls = string.Join(Environment.NewLine, imageCreationResponse.Result.Data.Select(x => x.Url));
                    Console.WriteLine($"Image ID: {id}");
                    Console.WriteLine($"Image URL: {urls}");
                }
            } while (!isFinished);
        }

        private HttpRequestMessage CreateHttpRequestMessage(string prompt, int? amount, int size)
        {
            // TODO
            string sizeValue = size + "x" + size;
            ImageRequestItemDto requestItem = new() 
            { 
                Prompt = prompt, 
                AmountOfImages = amount, 
                ImageSize = sizeValue 
            };
            string requestContent = JsonSerializer.Serialize(requestItem);
            HttpRequestMessage request = new(HttpMethod.Post, openAiOptions.ImageGenerationBaseUrl)
            {
                Content = new StringContent(requestContent, Encoding.UTF8, "application/json")
            };

            return request;
        }

        private async Task<string?> GetGenerationIdAsync(HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.EnsureSuccessStatusCode();
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            ImageResponseItemDto? response = JsonSerializer.Deserialize<ImageResponseItemDto>(content);
            return response?.Id;
        }

        private async Task<(bool Success, ImageResponseItemDto? Response)> GetImageCreationResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.EnsureSuccessStatusCode();
            string content = await httpResponseMessage.Content.ReadAsStringAsync();
            ImageResponseItemDto? response = JsonSerializer.Deserialize<ImageResponseItemDto>(content);
            return (response?.Status == "succeeded")
                ? (true, response)
                :(false, null);
        }

        private string GetCheckEndpoint(string? id) => $"{openAiOptions.ImageOperationBaseUrl}{id}?api-version=2023-06-01-preview";
    }
}
