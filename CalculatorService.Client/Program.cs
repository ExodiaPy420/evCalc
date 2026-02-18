/*var builder = Program.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/quotes", async ([FromServices] IHttpClientFactory factory) =>
{
    var client = factory.CreateClient();
    var response = await client.GetFromJsonAsync<GetQuotesResponse>("https://dummyjson.com/quotes");

    return response;
});

app.run();

 
 tbh all i can say is: i just don't know
 */


/*using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services
    .AddhttpClient("DummyJson", configureClient =>
    {
        configureClient.BaseAddress = new Uri("https://dummyjson.com");

        configureClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);

        configureClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your-token-here");
    });

var app = builder.Buil();

app.MapGet("/", async ([FromServices] IHttpClientFactory factory) =>
{
    var client = factory.CreateClient(name: "DummyJson"); //resolve clinet
    var response = await client.GetFromJsonAsync<GetQuotesResponse>("quotes");

    return response;
});


app.Run();

*/
using CalculatorService.Client;
using CalculatorService.Core.Models;

Console.WriteLine("calculator service client");

var baseUrl = "http://localhost:5271";
var client = new CalculatorApiClient(baseUrl);

Console.WriteLine("Enter numbers to add separated by space:");
var input = Console.ReadLine();
var numbers = input.Split(' ')
    .Select(x => double.TryParse(x, out var n) ? n : double.NaN)
    .Where(x => !double.IsNaN(x))
    .ToList();
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

Console.WriteLine("Press any key to exit program");
Console.ReadKey();

