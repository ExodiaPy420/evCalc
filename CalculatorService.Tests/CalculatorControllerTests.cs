using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using CalculatorService.Server.Controllers;
using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using CalculatorService.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CalculatorService.Tests
{
    [TestFixture]
    public class CalculatorControllerTests
    {
        private Mock<ICalculatorOperations> _mockCalculator;
        private Mock<IJournalService> _mockJournal;
        private Mock<ILogger<CalculatorController>> _mockLogger;
        private CalculatorController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCalculator = new Mock<ICalculatorOperations>();
            _mockJournal = new Mock<IJournalService>();
            _mockLogger = new Mock<ILogger<CalculatorController>>();

            _controller = new CalculatorController(
                _mockCalculator.Object,
                _mockJournal.Object,
                _mockLogger.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // Success case
        [Test]
        public void Add_WithValidRequest_ReturnsSum_And_DoesNotLog_To_Journal_IfNoTrackingId()
        {
            // Arrange
            var request = new AddRequest { Addends = new[] { 5.0, 5.0 } };
            _mockCalculator.Setup(c => c.Add(It.IsAny<IEnumerable<double>>())).Returns(10.0);
			// Act
			var result = _controller.Add(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var response = result.Value as AddResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Sum, Is.EqualTo(10.0));
            _mockJournal.Verify(j => j.Save(It.IsAny<string>(), It.IsAny<JournalEntry>()), Times.Never);
        }

        // Success case
        [Test]
        public void Add_WithTrackingId_SavesToJournal()
        {
            // Arrange
            var request = new AddRequest { Addends = new[] { 5.0, 5.0 } };
            _controller.Request.Headers["X-Evi-Tracking-Id"] = "Test-123";
            _mockCalculator.Setup(c => c.Add(request.Addends)).Returns(10.0);

            // Act
            var result = _controller.Add(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            _mockJournal.Verify(j => j.Save("Test-123", It.Is<JournalEntry>(e => e.Operation == "Sum" && e.Calculation == "5 + 5 = 10")), Times.Once);
        }

        // Exception case
        [Test]
        public void Add_WithNullAddends_ThrowsInvalidArgumentsException()
        {
            // Arrange
            var request = new AddRequest { Addends = null! };

            // Act & Assert
            Assert.Throws<InvalidArgumentsException>(() => _controller.Add(request));
        }

        // Success case
        [Test]
        public void Add_WithWhitespaceTrackingId_DoesNotSaveToJournal()
        {
            var request = new AddRequest { Addends = new[] { 1.0, 2.0 } };
            _controller.Request.Headers["X-Evi-Tracking-Id"] = "   ";
            _mockCalculator.Setup(c => c.Add(request.Addends)).Returns(3.0);

            var result = _controller.Add(request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            _mockJournal.Verify(j => j.Save(It.IsAny<string>(), It.IsAny<JournalEntry>()), Times.Never);
        }

        // Success case
        [Test]
        public void Sub_WithTrackingId_SavesToJournal()
        {
            var request = new SubRequest { Minuend = 10, Subtrahend = 3 };
            _controller.Request.Headers["X-Evi-Tracking-Id"] = "Sub-1";
            _mockCalculator.Setup(c => c.Subtract(request.Minuend, request.Subtrahend)).Returns(7.0);

            var result = _controller.Sub(request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            _mockJournal.Verify(j => j.Save("Sub-1", It.Is<JournalEntry>(e => e.Operation == "Sub" && e.Calculation == "10 - 3 = 7")), Times.Once);
        }

        // Exception case
        [Test]
        public void Mult_WithLessThanTwoFactors_ThrowsInvalidArgumentsException()
        {
            var request = new MultRequest { Factors = new[] { 2.0 } };
            Assert.Throws<InvalidArgumentsException>(() => _controller.Mult(request));
        }

        // Exception case
        [Test]
        public void Mult_WithNullRequest_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _controller.Mult(null!));
        }

        // Success case
        [Test]
        public void Div_WithValidRequest_ReturnsQuotientAndRemainder()
        {
            var request = new DivRequest { Dividend = -11, Divisor = 2 };
            _mockCalculator.Setup(c => c.Divide(request.Dividend, request.Divisor)).Returns((-6.0, -1.0));

            var result = _controller.Div(request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result!.Value as DivResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Quotient, Is.EqualTo(-6));
            Assert.That(response.Remainder, Is.EqualTo(-1));
        }

        // Exception case
        [Test]
        public void Div_WithNullRequest_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _controller.Div(null));
        }

        // Success case
        [Test]
        public void Sqrt_WithValidRequest_ReturnsSquareRoot()
        {
            var request = new SqrtRequest { Number = 0 };
            _mockCalculator.Setup(c => c.Sqrt(request.Number)).Returns(0);

            var result = _controller.Sqrt(request) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            var response = result!.Value as SqrtResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Square, Is.EqualTo(0));
        }

        // Exception case
        [Test]
        public void Sqrt_WithNullRequest_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _controller.Sqrt(null!));
        }

        // Exception case
        [Test]
        public void Sub_WithNullRequest_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _controller.Sub(null!));
        }
    }
}
