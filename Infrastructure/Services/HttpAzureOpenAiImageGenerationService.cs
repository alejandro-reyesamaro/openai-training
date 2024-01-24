using Azure.AI.OpenAI;
using Azure;
using ChGPTcmd.Application.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChGPTcmd.Models.Constants;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

namespace ChGPTcmd.Infrastructure.Services
{
    public class HttpAzureOpenAiImageGenerationService : ImageGenerationService
    {
        private HttpClient httpClient;
        private string endpoint;
        private ILogger<HttpOpenAiImageGenerationService> logger;

        internal record class ImageRequestItem(
            [property: JsonPropertyName("prompt")] string Prompt,
            [property: JsonPropertyName("n")] int? AmountOfImages,
            [property: JsonPropertyName("size")] string? ImageSize
        );

        internal record class ImageResponseItem(
            [property: JsonPropertyName("created")] int Created,
            [property: JsonPropertyName("expires")] int Expires,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("result")] Result Result,
            [property: JsonPropertyName("status")] string Status
        );

        internal record class Result(
            [property: JsonPropertyName("data")] Data[] Data);

        internal record class Data(
            [property: JsonPropertyName("url")] string Url);

        public HttpAzureOpenAiImageGenerationService(IConfiguration configuration, ILogger<HttpOpenAiImageGenerationService> logger)
        {
            httpClient = new HttpClient();
            string key = "d361bf99c57b440fa27a5e6ade2d9c39";//configuration.GetValue<string>("OpenAI:ApiKey") ?? throw new InvalidDataException("Failed to load OpenApi-Key");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add(
            "api-key",
            key);

            // set useragent header
            httpClient.DefaultRequestHeaders.Add(
                "x-ms-useragent",
                "AzureOpenAIImageConsole/0.0.1");
            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {key}");
            endpoint = "https://alex-openai-us-rsrc.openai.azure.com/openai/images/generations:submit?api-version=2023-06-01-preview";//configuration.GetValue<string>("OpenAI:ImageApiEndPoint") ?? throw new InvalidDataException("Failed to load OpenApi-Endpoint");
            this.logger = logger;
        }

        public async Task Post(string prompt)
        {
            /*
            StringContent content = BuildBody(prompt);
            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);
            string strResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(strResponse);
            */

            // create request
            HttpRequestMessage requestMessage = CreateHttpRequestMessage(prompt, 1, 1024);

            // make request
            HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
            
            // get id
            string? id = await GetGenerationIdAsync(response);

            Console.WriteLine();
            Console.WriteLine($"Your ID: {id}");
            // waiting...
            Console.WriteLine();
            Console.WriteLine("Image(s) will be generated...");
            // check if images are available
            var checkEndpoint = GetCheckEndpoint("alex-openai-us-rsrc", id);
            
            bool isFinished = false;
            do
            {
                await Task.Delay(2000);
                HttpResponseMessage checkResponse = await httpClient.GetAsync(checkEndpoint);
                (bool success, ImageResponseItem? imageCreationResponse) = await GetImageCreationResponseAsync(checkResponse);
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
            // translate size to size string
            string sizeValue = size + "x" + size;

            // create requestItem
            ImageRequestItem requestItem = new(
                prompt,
                amount,
                sizeValue);

            // serialize request body
            string requestContent = JsonSerializer.Serialize(requestItem);

            // get endpoint

            // create HttpRequestMessage
            HttpRequestMessage request = new(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    requestContent,
                    Encoding.UTF8,
                    "application/json")
            };

            // return request
            return request;
        }

        private async Task<string?> GetGenerationIdAsync(
        HttpResponseMessage httpResponseMessage)
        {
            // validate that the request was successful
            httpResponseMessage.EnsureSuccessStatusCode();

            // read the content
            string content = await httpResponseMessage.Content
                .ReadAsStringAsync();

            // deserialize the content
            ImageResponseItem? response = JsonSerializer.Deserialize<ImageResponseItem>(content);

            // return the image creation id
            return response?.Id;
        }

        private async Task<(bool Success, ImageResponseItem? Response)> GetImageCreationResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            // validate that the request was successful
            httpResponseMessage.EnsureSuccessStatusCode();

            // read the content
            string content = await httpResponseMessage.Content
                .ReadAsStringAsync();

            // deserialize the content
            ImageResponseItem? response = JsonSerializer.Deserialize<ImageResponseItem>(content);

            // check if status == "succeeded"
            if (response?.Status == "succeeded")
            {
                return (true, response);
            }

            // no validate response was send
            return (false, null);
        }

        // Get the endpoint for the image check
        private static string GetCheckEndpoint(string resource, string? id)
            => $"https://{resource}.openai.azure.com/" +
                $"openai/operations/images/{id}" +
                $"?api-version=2023-06-01-preview";
    }
}
