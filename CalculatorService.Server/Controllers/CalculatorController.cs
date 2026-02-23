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
            if (request == null || request.Subtrahends == null || !request.Subtrahends.Any())
                throw new InvalidArgumentsException("At least a number is required.");

            double result = _calculator.Subtract(request.Minuend, request.Subtrahends);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                var calculationString = request.Minuend + " - " + string.Join(" - ", request.Subtrahends);
                _journal.Save(trackingId, new JournalEntry("Subtract", $"{calculationString} = {result}"));
            }

            return Ok(new SubResponse { Result = result });
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
                _journal.Save(trackingId, new JournalEntry("Multiply", $"{string.Join(" * ", request.Factors)} = {result}"));
            }

            return Ok(new MultResponse { Result = result });
        }

        [HttpPost("div")]
        public IActionResult Div([FromBody] DivRequest request)
        {
            if (request?.Dividend == null || request.Divisor == null)
                throw new InvalidArgumentsException("Both dividend and divisor must be implemented.");

            var (quotient, remainder) = _calculator.Divide(request.Dividend, request.Divisor);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Divide", $"{request.Dividend} ÷ {request.Divisor} = {quotient}, remainder {remainder}"));
            }

            return Ok(new DivResponse { Quotient = quotient, Remainder = remainder });
        }

        [HttpPost("sqrt")]
        public IActionResult Sqrt([FromBody] SqrtRequest request)
        {
            if (request?.number == null)
                throw new InvalidArgumentsException("The square root input cannot be empty.");

            var result = _calculator.Sqrt(request.number);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Square root", $"√{request.number} = {result}"));
            }

            return Ok(new SqrtResponse { result = result });
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request)
        {
            if (request?.Addends == null || request.Addends.Count() < 2)
                throw new InvalidArgumentsException("At least two addends are required.");

            uint sum = _calculator.Add(request.Addends);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Add", $"{string.Join(" + ", request.Addends)} = {sum}"));
            }

            return Ok(new AddResponse { Sum = sum });
        }
    }
}