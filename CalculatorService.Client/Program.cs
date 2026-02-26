using CalculatorService.Client;
using CalculatorService.Core.Models;

namespace CalculatorService.ClientApp
{
    internal class Program
    {
        private static CalculatorApiClient _client = null!;
        private static string? _trackingId;

        static async Task Main(string[] args)
        {
            Initialize();
            await RunAsync();
            Exit();
        }

        // Initialization

        private static void Initialize()
        {
            Console.WriteLine("Evicertia Console-Based Calculator");

            var baseUrl = "http://localhost:5271";
            _client = new CalculatorApiClient(baseUrl);

            Console.WriteLine("Enter optional Tracking ID (leave empty for no tracking):");
            _trackingId = Console.ReadLine();
            Console.Clear();
        }

        // Main Menu Loop

        private static async Task RunAsync()
        {
            int selector;

            do
            {
                DisplayMenu();
                selector = ReadInt("Choose an operation: ");

                switch (selector)
                {
                    case 1: await AddAsync(); break;
                    case 2: await SubAsync(); break;
                    case 3: await MultAsync(); break;
                    case 4: await DivAsync(); break;
                    case 5: await SqrtAsync(); break;
                    case 6: await ShowJournalAsync(); break;
                    case 0: Console.WriteLine("Closing calculator..."); break;
                    default: Console.WriteLine("Invalid option."); break;
                }

            } while (selector != 0);
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("###########################################");
            Console.WriteLine("1. Addition");
            Console.WriteLine("2. Subtraction");
            Console.WriteLine("3. Multiplication");
            Console.WriteLine("4. Division");
            Console.WriteLine("5. Square root");
            Console.WriteLine("6. View calculation history");
            Console.WriteLine("0. Close calculator");
            Console.WriteLine("###########################################");
        }

        // Operations
      

        private static async Task AddAsync()
        {
            var numbers = ReadDoubleList("Enter numbers to add separated by space: ");

            if (numbers.Count < 2)
            {
                Console.WriteLine("At least two numbers are required.");
                return;
            }

            try
            {
                var result = await _client.AddAsync(numbers, _trackingId);
                Console.WriteLine($"Result: {result.Sum}");
                PrintTracking();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task SubAsync()
        {
            var minuend = ReadDouble("Enter the minuend: ");
            var subtrahend = ReadDouble("Enter the subtrahend: ");

            try
            {
                var result = await _client.SubAsync(minuend, subtrahend, _trackingId);
                Console.WriteLine($"Result: {result.Difference}");
                PrintTracking();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task MultAsync()
        {
            var numbers = ReadDoubleList("Enter numbers to multiply separated by space: ");

            if (numbers.Count < 2)
            {
                Console.WriteLine("At least two numbers required.");
                return;
            }

            try
            {
                var result = await _client.MultAsync(numbers, _trackingId);
                Console.WriteLine($"Result: {result.Product}");
                PrintTracking();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task DivAsync()
        {
            var dividend = ReadDouble("Enter dividend: ");

            double divisor;
            do
            {
                divisor = ReadDouble("Enter divisor: ");
                if (divisor == 0)
                    Console.WriteLine("Divisor cannot be zero.");
            } while (divisor == 0);

            try
            {
                var result = await _client.DivAsync(dividend, divisor, _trackingId);
                Console.WriteLine($"Quotient: {result.Quotient}");
                Console.WriteLine($"Remainder: {result.Remainder}");
                PrintTracking();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task SqrtAsync()
        {
            var number = ReadDouble("Enter number for square root: ");

            if (number < 0)
            {
                Console.WriteLine("Cannot calculate square root of negative number.");
                return;
            }

            try
            {
                var result = await _client.SqrtAsync(number, _trackingId);
                Console.WriteLine($"Result: {result.Square}");
                PrintTracking();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Journal

        private static async Task ShowJournalAsync()
        {
            if (string.IsNullOrWhiteSpace(_trackingId))
            {
                Console.WriteLine("No tracking ID configured — history not available.");
                return;
            }

            try
            {
                var entries = await _client.GetJournalAsync(_trackingId);

                if (!entries.Any())
                {
                    Console.WriteLine("No journal entries found.");
                    return;
                }

                Console.WriteLine($"Calculation history for {_trackingId}:");

                foreach (var entry in entries)
                {
                    Console.WriteLine($"  [{entry.Date:u}] {entry.Operation}: {entry.Calculation}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Helpers

        private static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            return int.TryParse(Console.ReadLine(), out var value) ? value : -1;
        }

        private static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out var number))
                    return number;

                Console.WriteLine("Invalid number. Try again.");
            }
        }

        private static List<double> ReadDoubleList(string prompt)
        {
            Console.Write(prompt);
            var input = Console.ReadLine() ?? "";

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var validNumbers = new List<double>();
            var invalidInputs = new List<string>();

            foreach (var part in parts)
            {
                if (double.TryParse(part, out var number))
                {
                    validNumbers.Add(number);
                }
                else
                {
                    invalidInputs.Add(part);
                }
            }

            if (invalidInputs.Any())
            {
                Console.WriteLine($"Invalid inputs detected will be omitted: {string.Join(", ", invalidInputs)}");
            }

            return validNumbers;
        }

        private static void PrintTracking()
        {
            if (!string.IsNullOrWhiteSpace(_trackingId))
                Console.WriteLine($"Tracking ID: {_trackingId}");
        }

        private static void Exit()
        {
            _client.Dispose();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}