using System.Net.Http.Json;
using CalculatorService.Core.Models;


namespace CalculatorService.Client
{
    public class CalculatorApiClient
    {
        private readonly HttpClient _httpClient;

        public CalculatorApiClient(string baseUrl)
        {
            _httpClient = new HttpClient
            { BaseAddress = new Uri(baseUrl) };
        }
        public async Task<AddResponse> AddAsync(IEnumerable<double> addends, string trackingId = null)
        {
            var request = new AddRequest { Addends = addends };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "calculator/add")
            {
                Content = JsonContent.Create(request)
            };

            if (!string.IsNullOrWhiteSpace(trackingId))
                httpRequest.Headers.Add("X-Evi-Tracking-Id", trackingId);

            var response = await _httpClient.SendAsync(httpRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {error}");
            }

            return await response.Content.ReadFromJsonAsync<AddResponse>();
        }

        public async Task<IEnumerable<JournalEntry>> GetJournalAsync(string trackingId)
        {
            if (string.IsNullOrWhiteSpace(trackingId))
                throw new ArgumentException("TRacking ID must be provided.", nameof(trackingId));

            var request = new { Id = trackingId };
            var response = await _httpClient.PostAsJsonAsync("journal/query", request);

            if(!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<JournalQueryResponse>();
            return result?.Operations ?? Enumerable.Empty<JournalEntry>();
        }

        public class JournalQueryResponse
        {
            public IEnumerable<JournalEntry> Operations { get; set; } = Enumerable.Empty<JournalEntry>();
        }
    }
}
