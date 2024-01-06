using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<AdventureWorks2019Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});


builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
builder.Services.AddHostedService<Worker>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
