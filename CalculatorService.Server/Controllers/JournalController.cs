using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CalculatorService.Server.Controllers
{
    [ApiController]
    [Route("journal")]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journal;
        private readonly ILogger<JournalController> _logger;

        public JournalController(IJournalService journal, ILogger<JournalController> logger)
        {
            _journal = journal;
            _logger = logger;
        }

        [HttpPost("query")]
        public IActionResult Query([FromBody] JournalQueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Id))
                return BadRequest(new { ErrorCode = "InvalidArguments", ErrorStatus = 400, ErrorMessage = "Tracking ID is required." });

            _logger.LogInformation("Journal query for tracking ID {TrackingId}", request.Id);

            var entries = _journal.GetOperations(request.Id);

            _logger.LogInformation("Journal query returned {Count} entries for {TrackingId}", entries.Count(), request.Id);
            return Ok(new JournalQueryResponse { Operations = entries });
        }


    }
}
