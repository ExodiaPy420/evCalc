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
                return BadRequest(new { ErrorMessage = "At least one subtrahend is required." });

            try
            {
                double result = _calculator.Subtract(request.Minuend, request.Subtrahends);
                string trackingId = Request.Headers["X-Evi-Tracking-Id"];

                /*if (!string.IsNullOrWhiteSpace(trackingId))
                {
                    _journal.Save(trackingId, new JournalEntry("Subtract", $"{string.Join(" + ", request.Minuend, request.Subtrahends)} = {result}"));


                }*/
                if (!string.IsNullOrWhiteSpace(trackingId))
                {
                    var calculationString = request.Minuend + " - " + string.Join(" - ", request.Subtrahends);
                    _journal.Save(trackingId, new JournalEntry("Subtract", $"{calculationString} = {result}"));
                }
                return Ok(new SubResponse { Result = result });
            } catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message}); 
            }
        }



        [HttpPost("mult")]
        public IActionResult Mult([FromBody] MultRequest request)
        {
            if (request?.Factors == null || request.Factors.Count() < 2)
                return BadRequest(new { ErrorMessage = "At least two factors are required." });

            try
            {
                double result = _calculator.Multiply(request.Factors);
                string trackingId = Request.Headers["X-Evi-Tracking-Id"];

                if (!string.IsNullOrWhiteSpace(trackingId))
                {
                    _journal.Save(trackingId, new JournalEntry("Multiply", $"{string.Join(" * ", request.Factors)} = {result}"));
                }

                return Ok(new MultResponse { Result = result });
            } catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }






        [HttpPost("add")]
        public IActionResult Add([FromBody] AddRequest request)
        {
            if (request?.Addends == null || request.Addends.Count() < 2)
                return BadRequest(new { ErrorMessage = "At least two addends are required." });

            try
            {
                uint sum = _calculator.Add(request.Addends);

                string trackingId = Request.Headers["X-Evi-Tracking-Id"];

                if(!string.IsNullOrWhiteSpace(trackingId))
                {

                    _journal.Save(
                        trackingId,
                        new JournalEntry(
                            "Add",
                            $"{string.Join(" + ", request.Addends)} = {sum}"
                        )
                    );




                    /*_journal.Save(trackingId, new JournalEntry
                    {
                        Operation = "Add",
                        Calculation = string.Join(" + ", request.Addends) + " = " + sum,
                        Date = DateTime.UtcNow
                    });*/

                }

                return Ok(new AddResponse { Sum = sum });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }


    }
}
