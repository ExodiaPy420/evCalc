using CalculatorService.Core.Interfaces;
using CalculatorService.Core.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddScoped<ICalculatorOperations, CalculatorOperations>();
builder.Services.AddSingleton<IJournalService, JournalService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


/*app.UseHttpsRedirection();*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
