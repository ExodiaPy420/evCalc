using CalculatorService.Core.Models;


namespace CalculatorService.Core.Interfaces
{
    public interface IJournalService
    {

        void Save(string trackingId, JournalEntry entry);

        IEnumerable<JournalEntry> GetOperations(string trackingId);

    }
}
