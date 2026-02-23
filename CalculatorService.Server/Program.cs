using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Services;
using CalculatorService.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICalculatorOperations, CalculatorOperations>();
builder.Services.AddSingleton<IJournalService, JournalService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

/*app.UseHttpsRedirection();*/

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();