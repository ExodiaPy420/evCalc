using CalculatorService.Core.Models;
using CalculatorService.Core.Services;
using NUnit.Framework;

namespace CalculatorService.Tests
{
    [TestFixture]
    public class JournalServiceTests
    {
        // Success case
        [Test]
        public void Save_WithWhitespaceTrackingId_DoesNotPersistEntry()
        {
            var filePath = Path.GetTempFileName();
            try
            {
                var service = new JournalService(filePath);
                service.Save("   ", new JournalEntry("Sum", "1 + 1 = 2"));

                var results = service.GetOperations("   ");
                Assert.That(results, Is.Empty);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        // Success case
        [Test]
        public void Save_WhenCalledConcurrently_PreservesAllEntries()
        {
            var filePath = Path.GetTempFileName();
            try
            {
                var service = new JournalService(filePath);
                var total = 200;

                Parallel.For(0, total, i =>
                {
                    service.Save("concurrent-id", new JournalEntry("Sum", $"{i} + 1 = {i + 1}"));
                });

                var entries = service.GetOperations("concurrent-id").ToList();
                Assert.That(entries.Count, Is.EqualTo(total));

                var calculations = entries.Select(e => e.Calculation).ToList();
                Assert.That(calculations.Distinct().Count(), Is.EqualTo(total));
                Assert.That(calculations, Does.Contain("0 + 1 = 1"));
                Assert.That(calculations, Does.Contain("199 + 1 = 200"));
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        // Success case
        [Test]
        public void JournalService_PersistsAndLoadsEntriesAcrossInstances()
        {
            var filePath = Path.GetTempFileName();
            try
            {
                var service1 = new JournalService(filePath);
                service1.Save("persist-id", new JournalEntry("Sum", "2 + 2 = 4"));
                service1.Save("persist-id", new JournalEntry("Sqrt", "√16 = 4"));

                var service2 = new JournalService(filePath);
                var loaded = service2.GetOperations("persist-id").ToList();

                Assert.That(loaded.Count, Is.EqualTo(2));
                Assert.That(loaded.Select(x => x.Operation), Is.EquivalentTo(new[] { "Sum", "Sqrt" }));
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        // Exception case
        [Test]
        public void JournalService_WithCorruptFile_StartsEmptyWithoutThrowing()
        {
            var filePath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(filePath, "{not-valid-json");

                Assert.DoesNotThrow(() =>
                {
                    var service = new JournalService(filePath);
                    var loaded = service.GetOperations("any-id");
                    Assert.That(loaded, Is.Empty);
                });
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
