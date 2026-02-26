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

        [Test]
        public void Add_WithValidRequest_ReturnsSumAndDoesNotLogToJournalIfNoTrackingId()
        {
            // Arrange
            var request = new AddRequest { Addends = new[] { 5.0, 5.0 } };
            _mockCalculator.Setup(c => c.Add(request.Addends)).Returns(10.0);

            // Act
            var result = _controller.Add(request) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var response = result.Value as AddResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Sum, Is.EqualTo(10.0));
            _mockJournal.Verify(j => j.Save(It.IsAny<string>(), It.IsAny<JournalEntry>()), Times.Never);
        }

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
            _mockJournal.Verify(j => j.Save("Test-123", It.Is<JournalEntry>(e => e.Operation == "Sum")), Times.Once);
        }

        [Test]
        public void Add_WithNullAddends_ThrowsInvalidArgumentsException()
        {
            // Arrange
            var request = new AddRequest { Addends = null };

            // Act & Assert
            Assert.Throws<InvalidArgumentsException>(() => _controller.Add(request));
        }
    }
}
