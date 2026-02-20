using CalculatorService.Client;
using CalculatorService.Core.Models;

Console.WriteLine("calculator service client");

var baseUrl = "http://localhost:5271";
var client = new CalculatorApiClient(baseUrl);

Console.WriteLine("Enter numbers to add separated by space:");
var input = Console.ReadLine() ?? string.Empty;

var numbers = new List<uint>();
foreach (var part in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
{
    if (uint.TryParse(part, out var n))
        numbers.Add(n);
}

Console.WriteLine("Enter optional Tracking ID. Leave empty for no tracking id record");
var trackingId = Console.ReadLine();

try
{
    var result = await client.AddAsync(numbers, trackingId);

    Console.WriteLine($"Result: {result.Sum}");

    if (!string.IsNullOrWhiteSpace(trackingId))
        Console.WriteLine($"Tracking ID: {trackingId}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}


Console.WriteLine("Do you want to see journal entries? Y/N");

string answer = Console.ReadLine();

answer = answer?.ToLowerInvariant();


//maybe a switch loop is not the best decision for this kind of user input choice but i can't think rn
/*switch(answer)
{
    case "y":
        //TODO
        Console.WriteLine("Enter the journal's tracking id you want to see:");
        var findTrackingId = Console.ReadLine();

        //Console.WriteLine(client.GetJournalAsync(findTrackingId).ToString());


        var journalEntries = await client.GetJournalAsync(findTrackingId);

        if (!journalEntries.Any())
        {
            Console.WriteLine("No journal entries found for this Tracking ID.");
        }
        else
        {
            Console.WriteLine($"Journal entries for Tracking ID: {findTrackingId}");

            foreach (var entry in journalEntries)
            {
                Console.WriteLine($"[{entry.Date}] {entry.Operation}: {entry.Calculation}");
            }
        }

        break;

    case "n":
        //TODO
        System.Environment.Exit(1);
        break;

    default:
        //loop over until user puts the right input
        break;
}*/



//idk what i was thinking while writing this i will change this asap cause this does the opposite of what i want to do i got confused with the brackets of the loop
string findTrackingId;

do
{
    Console.Write("Enter the tracking id you want to see the journal off:");
    findTrackingId = Console.ReadLine();

    if (findTrackingId != "y" && findTrackingId != "n")
    {
        Console.WriteLine("That is not a valid input. Please enter 'y'' or 'n'' ");
    }
} while (findTrackingId != "y" && findTrackingId != "n");

var journalEntries = await client.GetJournalAsync(findTrackingId);

if (!journalEntries.Any())
{
    Console.WriteLine("No journal entries found for this Tracking ID.");
}
else
{
    Console.WriteLine($"Journal entries for Tracking ID: {findTrackingId}");

    foreach (var entry in journalEntries)
    {
        Console.WriteLine($"[{entry.Date}] {entry.Operation}: {entry.Calculation}");
    }
}






/*try
{
    answer = Console.ReadLine();

} catch (Exception ex)
{
    Console.WriteLine("Debes responder solamente con  'y'  o con 'n' ");
}*/




Console.WriteLine("Press any key to exit program");
Console.ReadKey();

