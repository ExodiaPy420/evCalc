using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using System.Collections.Concurrent;


namespace CalculatorService.Core.Services
{
    public class JournalService : IJournalService
    {
       
        //private readonly ConcurrentDictionary<string, List<JournalEntry>> _journal = new();
        private readonly ConcurrentDictionary<string, ConcurrentQueue<JournalEntry>> _journal = new();


        public void Save(string trackingId, JournalEntry entry)
        {

            if (string.IsNullOrWhiteSpace(trackingId)) return;
            //_journal.AddOrUpdate(trackingId, new List<JournalEntry> { entry , (key, existingList) => { lock (existingList)}) idk what i was even trying


            _journal.GetOrAdd(trackingId, _ => new ConcurrentQueue<JournalEntry>()).Enqueue(entry);
            //_journal.AddOrUpdate(trackingId, new List<JournalEntry> { entry }, (key, existingList) =>
            /*{
                lock (existingList)
                {
                    existingList.GetOrAdd(key, _ => new ConcurrentQueue<JournalEntry>()).Enqueue(entry);
                }
                return existingList;
            }
        );*/

        }



        public IEnumerable<JournalEntry> GetOperations(string trackingId)
        {
            if (_journal.TryGetValue(trackingId, out var entries))
            {
                return entries;
            }

            return Enumerable.Empty<JournalEntry>();
        }


    }
}
