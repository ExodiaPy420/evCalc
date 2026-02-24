using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Models;
using System.Collections.Concurrent;
using System.Text.Json;


namespace CalculatorService.Core.Services
{
    public class JournalService : IJournalService
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<JournalEntry>> _journal = new();
        private readonly string _filePath;
        private readonly SemaphoreSlim _fileLock = new(1, 1);

        public JournalService(string filePath)
        {
            _filePath = filePath;
            LoadFromFile();
        }

        public void Save(string trackingId, JournalEntry entry)
        {
            if (string.IsNullOrWhiteSpace(trackingId)) return;

            _journal.GetOrAdd(trackingId, _ => new ConcurrentQueue<JournalEntry>()).Enqueue(entry);
            SaveToFile();
        }

        public IEnumerable<JournalEntry> GetOperations(string trackingId)
        {
            if (_journal.TryGetValue(trackingId, out var entries))
            {
                return entries;
            }

            return Enumerable.Empty<JournalEntry>();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_filePath)) return;

            try
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, List<JournalEntry>>>(json);

                if (data == null) return;

                foreach (var kvp in data)
                {
                    var queue = new ConcurrentQueue<JournalEntry>(kvp.Value);
                    _journal[kvp.Key] = queue;
                }
            }
            catch
            {
                // Corrupt or unreadable file — start empty
            }
        }

        private void SaveToFile()
        {
            _fileLock.Wait();
            try
            {
                var snapshot = _journal.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToList()
                );

                var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
                var tempPath = _filePath + ".tmp";
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, _filePath, overwrite: true);
            }
            finally
            {
                _fileLock.Release();
            }
        }
    }
}
