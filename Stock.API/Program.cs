using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();
    configurator.AddConsumer<StockRollBackMessageConsumer>();

    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue,
       e => e.ConfigureConsumer<StockRollBackMessageConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDBService>();

var app = builder.Build();

using var scope = builder.Services.BuildServiceProvider().CreateScope();

var mongoDbService = scope.ServiceProvider.GetRequiredService<MongoDBService>();


if(!await (await mongoDbService.GetCollection<Stock.API.Models.Stock>() .FindAsync(c=> true)).AnyAsync())
{
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock
    {
        ProductId = 1,
        Count = 200
    });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock
    {
        ProductId = 2,
        Count = 300
    });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock
    {
        ProductId = 3,
        Count = 50
    });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock
    {
        ProductId = 4,
        Count = 10
    });
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock
    {
        ProductId = 5,
        Count = 60
    });
}

app.Run();

