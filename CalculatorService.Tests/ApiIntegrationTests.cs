using System.Net;
using System.Net.Http.Json;
using System.Collections.Concurrent;
using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using CalculatorService.Core.Services;
using CalculatorService.Server.Controllers;
using CalculatorService.Server.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CalculatorService.Tests
{
    [TestFixture]
    public class ApiIntegrationTests
    {
        // Success case
        [Test]
        public async Task Add_Endpoint_UsesRoutingBindingAndSerialization()
        {
            using var server = CreateServer();
            using var client = server.CreateClient();

            var response = await client.PostAsJsonAsync("/calculator/add", new AddRequest { Addends = new[] { 3.0, 3.0, 2.0 } });
            var body = await response.Content.ReadFromJsonAsync<AddResponse>();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body, Is.Not.Null);
            Assert.That(body!.Sum, Is.EqualTo(8));
        }

        // Exception case
        [Test]
        public async Task ExceptionMiddleware_ReturnsStandard400_ForBusinessExceptions()
        {
            using var server = CreateServer();
            using var client = server.CreateClient();

            var response = await client.PostAsJsonAsync("/calculator/add", new AddRequest { Addends = new[] { 1.0 } });
            var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(body, Is.Not.Null);
            Assert.That(body!.ErrorCode, Is.EqualTo("InvalidArguments"));
            Assert.That(body.ErrorStatus, Is.EqualTo(400));
            Assert.That(body.ErrorMessage, Is.Not.Empty);
        }

        // Exception case
        [Test]
        public async Task ExceptionMiddleware_ReturnsStandard500_ForUnhandledExceptions()
        {
            using var server = CreateServer(new ThrowingCalculatorOperations());
            using var client = server.CreateClient();

            var response = await client.PostAsJsonAsync("/calculator/add", new AddRequest { Addends = new[] { 2.0, 2.0 } });
            var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(body, Is.Not.Null);
            Assert.That(body!.ErrorCode, Is.EqualTo("InternalError"));
            Assert.That(body.ErrorStatus, Is.EqualTo(500));
            Assert.That(body.ErrorMessage, Is.Not.Empty);
        }

        // Exception case
        [Test]
        public async Task Journal_Query_WithMissingId_ReturnsBadRequest()
        {
            using var server = CreateServer();
            using var client = server.CreateClient();

            var response = await client.PostAsJsonAsync("/journal/query", new JournalQueryRequest { Id = "" });
            var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(body, Is.Not.Null);
            Assert.That(body!.ErrorCode, Is.EqualTo("InvalidArguments"));
            Assert.That(body.ErrorStatus, Is.EqualTo(400));
            Assert.That(body.ErrorMessage, Is.Not.Empty);
        }

        // Success case
        [Test]
        public async Task Add_WithTrackingId_ThenQueryJournal_ReturnsTrackedOperation()
        {
            using var server = CreateServer();
            using var client = server.CreateClient();

            var addRequest = new HttpRequestMessage(HttpMethod.Post, "/calculator/add")
            {
                Content = JsonContent.Create(new AddRequest { Addends = new[] { 5.0, 7.0 } })
            };
            addRequest.Headers.Add("X-Evi-Tracking-Id", "integration-track");

            var addResponse = await client.SendAsync(addRequest);
            var queryResponse = await client.PostAsJsonAsync("/journal/query", new JournalQueryRequest { Id = "integration-track" });
            var queryBody = await queryResponse.Content.ReadFromJsonAsync<JournalQueryResponse>();

            Assert.That(addResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(queryResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(queryBody, Is.Not.Null);
            Assert.That(queryBody!.Operations.Count(), Is.EqualTo(1));

            var entry = queryBody.Operations.Single();
            Assert.That(entry.Operation, Is.EqualTo("Sum"));
            Assert.That(entry.Calculation, Is.EqualTo("5 + 7 = 12"));
        }

        private static TestServer CreateServer(ICalculatorOperations? calculator = null)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddSingleton<ICalculatorOperations>(calculator ?? new CalculatorOperations());
                    services.AddSingleton<IJournalService, InMemoryJournalService>();
                    services.AddControllers().AddApplicationPart(typeof(CalculatorController).Assembly);
                })
                .Configure(app =>
                {
                    app.UseMiddleware<ExceptionMiddleware>();
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                });

            return new TestServer(builder);
        }

        private sealed class InMemoryJournalService : IJournalService
        {
            private readonly ConcurrentDictionary<string, List<JournalEntry>> _entries = new();

            public void Save(string trackingId, JournalEntry entry)
            {
                if (string.IsNullOrWhiteSpace(trackingId))
                {
                    return;
                }

                var list = _entries.GetOrAdd(trackingId, _ => new List<JournalEntry>());
                lock (list)
                {
                    list.Add(entry);
                }
            }

            public IEnumerable<JournalEntry> GetOperations(string trackingId)
            {
                if (_entries.TryGetValue(trackingId, out var list))
                {
                    lock (list)
                    {
                        return list.ToList();
                    }
                }

                return Enumerable.Empty<JournalEntry>();
            }
        }

        private sealed class ThrowingCalculatorOperations : ICalculatorOperations
        {
            public double Add(IEnumerable<double> addends) => throw new Exception("Unexpected test exception");
            public (double Quotient, double Remainder) Divide(double dividend, double divisor) => (0, 0);
            public double Multiply(IEnumerable<double> factors) => 0;
            public double Sqrt(double number) => 0;
            public double Subtract(double minuend, double subtrahend) => 0;
        }

        private sealed class ErrorResponse
        {
            public string ErrorCode { get; set; } = string.Empty;
            public int ErrorStatus { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }
    }
}
