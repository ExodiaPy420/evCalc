using NUnit.Framework;
using CalculatorService.Core.Services;
using CalculatorService.Core.Exceptions;

namespace CalculatorService.Tests
{
    [TestFixture]
    public class CalculatorOperationsTests
    {
        private CalculatorOperations _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new CalculatorOperations();
        }

        [Test]
        public void Add_WithValidNumbers_ReturnsCorrectSum()
        {
            var result = _calculator.Add(new[] { 1.0, 2.0, 3.0 });
            Assert.That(result, Is.EqualTo(6.0));
        }

        [Test]
        public void Add_WithLessThanTwoNumbers_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _calculator.Add(new[] { 1.0 }));
        }

        [Test]
        public void Divide_WithValidNumbers_ReturnsCorrectQuotientAndRemainder()
        {
            var (quotient, remainder) = _calculator.Divide(11, 2);
            Assert.That(quotient, Is.EqualTo(5));
            Assert.That(remainder, Is.EqualTo(1));
        }

        [Test]
        public void Divide_ByZero_ThrowsDivisionByZeroException()
        {
            Assert.Throws<DivisionByZeroException>(() => _calculator.Divide(10, 0));
        }

        [Test]
        public void Sqrt_WithPositiveNumber_ReturnsSquareRoot()
        {
            var result = _calculator.Sqrt(16);
            Assert.That(result, Is.EqualTo(4));
        }

        [Test]
        public void Sqrt_WithNegativeNumber_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _calculator.Sqrt(-16));
        }
    }
}
