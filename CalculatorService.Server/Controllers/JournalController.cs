using CalculatorService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CalculatorService.Server.Controllers
{
    [Route("journal")]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journal;

        public JournalController(IJournalService journal)
        {
            _journal = journal;
        }

        public class JournalQueryRequest
        {
            public string Id { get; set; } = string.Empty;
        }

        [HttpPost("query")]
        public IActionResult Query([FromBody] JournalQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Id))
                return BadRequest(new { ErrorMessage = "Tracking ID is required." });

            try
            {
                var entries = _journal.GetOperations(request.Id);
                return Ok(new { Operations = entries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = ex.Message });
            }
        }

        
    }
}
