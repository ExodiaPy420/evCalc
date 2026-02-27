using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using CalculatorService.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace CalculatorService.Tests
{
    [TestFixture]
    public class JournalControllerTests
    {
        private Mock<IJournalService> _mockJournal = null!;
        private Mock<ILogger<JournalController>> _mockLogger = null!;
        private JournalController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockJournal = new Mock<IJournalService>();
            _mockLogger = new Mock<ILogger<JournalController>>();
            _controller = new JournalController(_mockJournal.Object, _mockLogger.Object);
        }

        // Exception case
        [Test]
        public void Query_WithMissingId_ReturnsBadRequestWithStandardShape()
        {
            var result = _controller.Query(new JournalQueryRequest { Id = "" }) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            var payload = ToErrorResponse(result!.Value);
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload!.ErrorCode, Is.EqualTo("InvalidArguments"));
            Assert.That(payload.ErrorStatus, Is.EqualTo(400));
            Assert.That(payload.ErrorMessage, Is.Not.Empty);
        }

        // Success case
        [Test]
        public void Query_WithValidId_ReturnsOperations()
        {
            var entries = new[]
            {
                new JournalEntry("Sum", "1 + 2 = 3"),
                new JournalEntry("Mul", "2 * 3 = 6")
            };

            _mockJournal.Setup(j => j.GetOperations("track-1")).Returns(entries);

            var result = _controller.Query(new JournalQueryRequest { Id = "track-1" }) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result!.Value as JournalQueryResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Operations.Count(), Is.EqualTo(2));
        }

        private static ErrorResponse? ToErrorResponse(object? value)
        {
            if (value == null)
            {
                return null;
            }

            var json = JsonSerializer.Serialize(value);
            return JsonSerializer.Deserialize<ErrorResponse>(json);
        }

        private sealed class ErrorResponse
        {
            public string ErrorCode { get; set; } = string.Empty;
            public int ErrorStatus { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }
    }
}
