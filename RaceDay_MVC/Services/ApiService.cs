using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay_MVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient(string? token = null)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<T?> GetAsync<T>(
            string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            return await client.GetFromJsonAsync<T>(endpoint);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(
            string endpoint, T data, string? token = null)
        {
            var client = CreateClient(token);
            return await client.PostAsJsonAsync(endpoint, data);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(
            string endpoint, T data, string? token = null)
        {
            var client = CreateClient(token);
            return await client.PutAsJsonAsync(endpoint, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(
            string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            return await client.DeleteAsync(endpoint);
        }
    }
}