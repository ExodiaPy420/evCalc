namespace CalculatorService.Core.Models
{
    public class JournalQueryResponse
    {
        public IEnumerable<JournalEntry> Operations { get; set; } = Enumerable.Empty<JournalEntry>();
    }
}
