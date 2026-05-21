using Api.Extensions;
using Api.Hubs;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Nuestras capas
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// CORS
builder.Services.ConfigureCors();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("CorsPolicy");

// Servir archivos estáticos desde wwwroot (el HTML y JS)
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Registrar el Hub en la ruta /votingHub
app.MapHub<VotingHub>("/votingHub");

app.Run();