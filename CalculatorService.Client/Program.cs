using CalculatorService.Client;
using CalculatorService.Core.Models;
using System.Runtime.CompilerServices;

Console.WriteLine("calculator service client");

var baseUrl = "http://localhost:5271";
var client = new CalculatorApiClient(baseUrl);



Console.WriteLine("Evicertia Console-Based Calculator");

Console.WriteLine("");


Console.WriteLine("Enter optional Tracking ID. Leave empty for no tracking id record");
var trackingId = Console.ReadLine();
Console.Clear();

int selector;
do
{
    Console.WriteLine("Choose the operation you desire to perform:");
    Console.WriteLine("###########################################");
    Console.WriteLine();
    Console.WriteLine("1. Addition");
    Console.WriteLine("2. Subtraction");
    Console.WriteLine("3. Multiplication");
    Console.WriteLine("4. Division");
    Console.WriteLine("5. Square root");
    Console.WriteLine("0. Close calculator");
    Console.WriteLine();
    Console.WriteLine("############################################");

    int.TryParse(Console.ReadLine(), out selector);


    switch (selector)
    {
        case 1:
            await Add(client, trackingId);
            break;

        case 2:
            await Sub(client, trackingId);
            break;

        case 3:
            //TODO MULT
            break;

        case 4:
            //TODO DIV
            break;

        case 5:
            //TODO SQRT
            break;

        case 0:
            Console.WriteLine("Closing calculator....");
            break;
    }
} while (selector != 0);


static async Task Sub(CalculatorApiClient client, string? trackingId) 
{
    Console.WriteLine("Enter the minuend: ");
    var input = Console.ReadLine() ?? string.Empty;

    double.TryParse(input, out var minuend);

    Console.WriteLine("Enter the subtrahends separated by a white space:");
    var inpSubtrahends = Console.ReadLine() ?? string.Empty;

    var subtrahends = new List<double>();
    foreach(var part in inpSubtrahends.Split(' ', StringSplitOptions.RemoveEmptyEntries))
    {
        if (double.TryParse(part, out var n))
            subtrahends.Add(n);
    }

    try
    {
        var result = await client.SubAsync(minuend, subtrahends, trackingId);

        Console.WriteLine($"Result: {result.Result}");

        if (!string.IsNullOrWhiteSpace(trackingId))
            Console.WriteLine($"Tracking ID: {trackingId}");
    } catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


//maybe we should remove async from this, just maybe
static async Task Add(CalculatorApiClient client, string? trackingId)
{
    Console.WriteLine("Enter numbers to add separated by space:");
    var input = Console.ReadLine() ?? string.Empty;

    var numbers = new List<uint>();
    foreach (var part in input.Split(' ', StringSplitOptions.RemoveEmptyEntries))
    {
        if (uint.TryParse(part, out var n))
            numbers.Add(n);
    }


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

}







Console.WriteLine("Do you want to see journal entries? Y/N");
string answer = Console.ReadLine()?.Trim().ToLowerInvariant();

if (answer == "y" || answer == "yes")
{
    string findTrackingId;
    do
    {
        Console.Write("Enter the tracking ID you want to see the journal of: ");
        findTrackingId = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(findTrackingId))
        {
            Console.WriteLine("Tracking ID cannot be empty. Please try again.");
        }
        
    }
    while (string.IsNullOrWhiteSpace(findTrackingId));

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
}
else
{
    Console.WriteLine("Okay, no journal entries will be shown.");
}

Console.WriteLine("Press any key to exit program");
Console.ReadKey();



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





/*try
{
    answer = Console.ReadLine();

} catch (Exception ex)
{
    Console.WriteLine("Debes responder solamente con  'y'  o con 'n' ");
}*/



