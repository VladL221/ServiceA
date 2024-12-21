using ServiceA.Interfaces;
using ServiceA.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMessageStrategy, JsonMessageStrategy>();
builder.Services.AddSingleton<IWebSocketClient, WebSocketClientService>();
builder.Services.AddScoped<IGraphService, GraphService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
// test endpint
app.MapGet("/test", () => "Service A is running!");

app.Logger.LogInformation("ServiceA starting on http://localhost:5001");
app.Run();