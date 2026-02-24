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
        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ICalculatorOperations calculator, IJournalService journal, ILogger<CalculatorController> logger)
        {
            _calculator = calculator;
            _journal = journal;
            _logger = logger;
        }

        [HttpPost("sub")]
        public IActionResult Sub([FromBody] SubRequest request)
        {
            if (request == null)
                throw new InvalidArgumentsException("Both minuend and subtrahend are required.");

            _logger.LogInformation("Sub requested: {Minuend} - {Subtrahend}", request.Minuend, request.Subtrahend);

            double result = _calculator.Subtract(request.Minuend, request.Subtrahend);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Sub", $"{request.Minuend} - {request.Subtrahend} = {result}"));
                _logger.LogDebug("Journal entry saved for tracking ID {TrackingId}", trackingId);
            }

            _logger.LogInformation("Sub result: {Result}", result);
            return Ok(new SubResponse { Difference = result });
        }

        [HttpPost("mult")]
        public IActionResult Mult([FromBody] MultRequest request)
        {
            if (request?.Factors == null || request.Factors.Count() < 2)
                throw new InvalidArgumentsException("At least two factors are required.");

            _logger.LogInformation("Mult requested: {Factors}", string.Join(", ", request.Factors));

            double result = _calculator.Multiply(request.Factors);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Mul", $"{string.Join(" * ", request.Factors)} = {result}"));
                _logger.LogDebug("Journal entry saved for tracking ID {TrackingId}", trackingId);
            }

            _logger.LogInformation("Mult result: {Result}", result);
            return Ok(new MultResponse { Product = result });
        }

        [HttpPost("div")]
        public IActionResult Div([FromBody] DivRequest request)
        {
            if (request == null)
                throw new InvalidArgumentsException("Both dividend and divisor are required.");

            _logger.LogInformation("Div requested: {Dividend} / {Divisor}", request.Dividend, request.Divisor);

            var (quotient, remainder) = _calculator.Divide(request.Dividend, request.Divisor);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Div", $"{request.Dividend} / {request.Divisor} = {quotient} remainder {remainder}"));
                _logger.LogDebug("Journal entry saved for tracking ID {TrackingId}", trackingId);
            }

            _logger.LogInformation("Div result: Quotient={Quotient}, Remainder={Remainder}", quotient, remainder);
            return Ok(new DivResponse { Quotient = quotient, Remainder = remainder });
        }

        [HttpPost("sqrt")]
        public IActionResult Sqrt([FromBody] SqrtRequest request)
        {
            if (request == null)
                throw new InvalidArgumentsException("A number is required.");

            _logger.LogInformation("Sqrt requested: √{Number}", request.Number);

            var result = _calculator.Sqrt(request.Number);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Sqrt", $"√{request.Number} = {result}"));
                _logger.LogDebug("Journal entry saved for tracking ID {TrackingId}", trackingId);
            }

            _logger.LogInformation("Sqrt result: {Result}", result);
            return Ok(new SqrtResponse { Square = result });
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request)
        {
            if (request?.Addends == null || request.Addends.Count() < 2)
                throw new InvalidArgumentsException("At least two addends are required.");

            _logger.LogInformation("Add requested: {Addends}", string.Join(", ", request.Addends));

            double sum = _calculator.Add(request.Addends);

            string trackingId = Request.Headers["X-Evi-Tracking-Id"];
            if (!string.IsNullOrWhiteSpace(trackingId))
            {
                _journal.Save(trackingId, new JournalEntry("Sum", $"{string.Join(" + ", request.Addends)} = {sum}"));
                _logger.LogDebug("Journal entry saved for tracking ID {TrackingId}", trackingId);
            }

            _logger.LogInformation("Add result: {Sum}", sum);
            return Ok(new AddResponse { Sum = sum });
        }
    }
}