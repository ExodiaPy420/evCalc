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

        // Success case
        [Test]
        public void Add_WithValidNumbers_ReturnsCorrectSum()
        {
            var result = _calculator.Add(new[] { 1.0, 2.0, 3.0 });
            Assert.That(result, Is.EqualTo(6.0));
        }

        // Exception case
        [Test]
        public void Add_WithLessThanTwoNumbers_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _calculator.Add(new[] { 1.0 }));
        }

        // Success case
        [Test]
        public void Add_WithFloatingPointNumbers_ReturnsExpectedSumWithinTolerance()
        {
            var result = _calculator.Add(new[] { 0.1, 0.2, 0.3 });
            Assert.That(result, Is.EqualTo(0.6).Within(1e-12));
        }

        // Exception case
        [Test]
        public void Multiply_WithLessThanTwoNumbers_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _calculator.Multiply(new[] { 2.0 }));
        }

        // Success case
        [Test]
        public void Multiply_WithLargeValues_ReturnsExpectedProduct()
        {
            var result = _calculator.Multiply(new[] { 1e100, 1e100 });
            Assert.That(double.IsFinite(result), Is.True);
            Assert.That(result, Is.EqualTo(1e200).Within(1e188));
        }

        // Success case
        [Test]
        public void Divide_WithValidNumbers_ReturnsCorrectQuotientAndRemainder()
        {
            var (quotient, remainder) = _calculator.Divide(11, 2);
            Assert.That(quotient, Is.EqualTo(5));
            Assert.That(remainder, Is.EqualTo(1));
        }

        // Exception case
        [Test]
        public void Divide_ByZero_ThrowsDivisionByZeroException()
        {
            Assert.Throws<DivisionByZeroException>(() => _calculator.Divide(10, 0));
        }

        // Success case
        [Test]
        public void Divide_WithNegativeDividend_ReturnsFloorQuotientAndRemainder()
        {
            var (quotient, remainder) = _calculator.Divide(-11, 2);
            Assert.That(quotient, Is.EqualTo(-6));
            Assert.That(remainder, Is.EqualTo(-1));
        }

        // Success case
        [Test]
        public void Sqrt_WithPositiveNumber_ReturnsSquareRoot()
        {
            var result = _calculator.Sqrt(16);
            Assert.That(result, Is.EqualTo(4));
        }

        // Exception case
        [Test]
        public void Sqrt_WithNegativeNumber_ThrowsInvalidArgumentsException()
        {
            Assert.Throws<InvalidArgumentsException>(() => _calculator.Sqrt(-16));
        }

        // Success case
        [Test]
        public void Sqrt_WithZero_ReturnsZero()
        {
            var result = _calculator.Sqrt(0);
            Assert.That(result, Is.EqualTo(0));
        }
    }
}
