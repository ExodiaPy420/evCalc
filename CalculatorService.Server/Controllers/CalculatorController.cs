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
