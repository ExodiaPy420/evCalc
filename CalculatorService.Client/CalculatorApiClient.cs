using System.Net.Http.Json;
using CalculatorService.Core.Models;
using RestSharp;


namespace CalculatorService.Client
{
    public class CalculatorApiClient : IDisposable
    {
        private readonly RestClient _client;

        


		public CalculatorApiClient(string baseUrl)
        {
			var options = new RestClientOptions(baseUrl) { Timeout = TimeSpan.Parse("0.00:00:05") };

			_client = new RestClient(options);
		}
        public async Task<AddResponse> AddAsync(IEnumerable<double> addends, string trackingId = null)
        {
            var requestBody = new AddRequest { Addends = addends };

            //var httpRequest = new HttpRequestMessage(HttpMethod.Post, "calculator/add");

            var request = new RestRequest("calculator/add", Method.Post).AddJsonBody(requestBody);

            /*{
                Content = JsonContent.Create(request)
            };*/

            if (!string.IsNullOrWhiteSpace(trackingId))
                request.AddHeader("X-Evi-Tracking-Id", trackingId);

            var response = await _client.ExecuteAsync<AddResponse>(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.Content}");
            }

            if (response.Data is null)
                throw new Exception("API Error: Empty response body.");

            return response.Data;

            //return await response.Content.ReadFromJsonAsync<AddResponse>();
        }


        public async Task<DivResponse> DivAsync(double dividend, double divisor, string trackingId = null)
        {
            var requestBody = new DivRequest { Dividend = dividend, Divisor = divisor };

            var request = new RestRequest("calculator/div", Method.Post).AddJsonBody(requestBody);

			/*var httpRequest = new HttpRequestMessage(HttpMethod.Post, "calculator/div")
            {
                Content = JsonContent.Create(request)
            };*/

            if (!string.IsNullOrWhiteSpace(trackingId))
                request.AddHeader("X-Evi-Tracking-Id", trackingId);

            var response = await _client.ExecuteAsync<DivResponse>(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.Content}");
            }

            if (response.Data is null)
                throw new Exception("API Error: Empty response body.");

			return response.Data;
        }

        public async Task<SqrtResponse> SqrtAsync(double number, string trackingId = null)
        {

            var requestBody = new SqrtRequest { Number = number };
            //var request = new SqrtRequest { Number = number };

            var request = new RestRequest("calculator/sqrt", Method.Post).AddJsonBody(requestBody);
            /*{
                Content = JsonContent.Create(request)
            };*/

			if (!string.IsNullOrWhiteSpace(trackingId))
                request.AddHeader("X-Evi-Tracking-Id", trackingId);

			var response = await _client.ExecuteAsync<SqrtResponse>(request);

			if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.Content}");
            }

            if (response.Data is null)
                throw new Exception("API Error: Empty response body.");

            return response.Data;
        }

        public async Task<MultResponse> MultAsync(IEnumerable<double> factors, string trackingId = null)
        {
            //var request = new MultRequest { Factors = factors };
            var requestBody = new MultRequest { Factors = factors };

            var request = new RestRequest("calculator/mult", Method.Post).AddJsonBody(requestBody);

			/*var httpRequest = new HttpRequestMessage(HttpMethod.Post, "calculator/mult")
            {
                Content = JsonContent.Create(request)
            };*/

            if (!string.IsNullOrWhiteSpace(trackingId))
                request.AddHeader("X-Evi-Tracking-Id", trackingId);

			var response = await _client.ExecuteAsync<MultResponse>(request);

			if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.Content}");
            }
            if (response.Data is null)
                throw new Exception("API Error: Empty response body.");

			return response.Data;
        }

        public async Task<SubResponse> SubAsync(double minuend, double subtrahend, string trackingId = null)
        {
            var requestBody = new SubRequest { Minuend = minuend, Subtrahend = subtrahend };

			//var request = new SubRequest { Minuend = minuend, Subtrahend = subtrahend };

            var request = new RestRequest("calculator/sub", Method.Post).AddJsonBody(requestBody);

			/*var httpRequest = new HttpRequestMessage(HttpMethod.Post, "calculator/sub")
            {
                Content = JsonContent.Create(request)
            };*/

            if (!string.IsNullOrWhiteSpace(trackingId))
                request.AddHeader("X-Evi-Tracking-Id", trackingId);

            //var response = await _client.SendAsync(httpRequest);
            var response = await _client.ExecuteAsync<SubResponse>(request);

			if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.Content}");
            }

            if (response.Data is null)
                throw new Exception("API Error: Empty response body.");


			return response.Data;
        }


        public async Task<IEnumerable<JournalEntry>> GetJournalAsync(string trackingId)
        {
            if (string.IsNullOrWhiteSpace(trackingId))
                throw new ArgumentException("Tracking ID must be provided.", nameof(trackingId));


            var requestBody = new JournalQueryRequest { Id = trackingId };

            var request = new RestRequest("journal/query", Method.Post).AddJsonBody(requestBody);

			//var request = new JournalQueryRequest { Id = trackingId };
            //var response = await _client.PostAsJsonAsync("journal/query", request);

            var response = await _client.ExecuteAsync<JournalQueryResponse>(request);

			if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {response.StatusCode} - {response.Content}");
            }

            if (response.Data is null)
                throw new Exception("API Error: Empty response body.");

            //var result = await response.Content.ReadFromJsonAsync<JournalQueryResponse>();
            return response.Data.Operations ?? Enumerable.Empty<JournalEntry>();
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
