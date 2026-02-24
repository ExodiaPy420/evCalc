using CalculatorService.Core.Exceptions;
using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalculatorService.Server.Controllers
{
    [ApiController]
    [Route("calculator")]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorOperations _calculator;
        private readonly IJournalService _journal;

        public CalculatorController(ICalculatorOperations calculator, IJournalService journal)
        {
            _calculator = calculator;
            _journal = journal;
        }

        [HttpPost("sub")]
        public IActionResult Sub([FromBody] SubRequest request)
        {
            if (request == null)
                throw new InvalidArgumentsException("Both minuend and subtrahend are required.");

            double result = _calculator.Subtract(request.Minuend, request.Subtrahend);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Sub", $"{request.Minuend} - {request.Subtrahend} = {result}"));
            }

            return Ok(new SubResponse { Difference = result });
        }

        [HttpPost("mult")]
        public IActionResult Mult([FromBody] MultRequest request)
        {
            if (request?.Factors == null || request.Factors.Count() < 2)
                throw new InvalidArgumentsException("At least two factors are required.");

            double result = _calculator.Multiply(request.Factors);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Mul", $"{string.Join(" * ", request.Factors)} = {result}"));
            }

            return Ok(new MultResponse { Product = result });
        }

        [HttpPost("div")]
        public IActionResult Div([FromBody] DivRequest request)
        {
            if (request == null)
                throw new InvalidArgumentsException("Both dividend and divisor are required.");

            var (quotient, remainder) = _calculator.Divide(request.Dividend, request.Divisor);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Div", $"{request.Dividend} / {request.Divisor} = {quotient} remainder {remainder}"));
            }

            return Ok(new DivResponse { Quotient = quotient, Remainder = remainder });
        }

        [HttpPost("sqrt")]
        public IActionResult Sqrt([FromBody] SqrtRequest request)
        {
            if (request == null)
                throw new InvalidArgumentsException("A number is required.");

            var result = _calculator.Sqrt(request.Number);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Sqrt", $"√{request.Number} = {result}"));
            }

            return Ok(new SqrtResponse { Square = result });
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request)
        {
            if (request?.Addends == null || request.Addends.Count() < 2)
                throw new InvalidArgumentsException("At least two addends are required.");

            double sum = _calculator.Add(request.Addends);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Sum", $"{string.Join(" + ", request.Addends)} = {sum}"));
            }

            return Ok(new AddResponse { Sum = sum });
        }
    }
}