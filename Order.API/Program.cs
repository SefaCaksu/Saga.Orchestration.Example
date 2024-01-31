using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Order.API.Models.Contexts;
using Order.API.ViewModels;
using Shared;
using Shared.OrderEvents;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));


builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCompletedEventConsumer>();
    configurator.AddConsumer<OrderFailedEventConsumer>();

    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderCompletedEventQueue,
          e => e.ConfigureConsumer<OrderCompletedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderFailedEventQueue,
         e => e.ConfigureConsumer<OrderFailedEventConsumer>(context));
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.MapPost("/create-order", async (CreateOrderVM model,
                                    OrderApiContext context,
                                    ISendEndpointProvider sendEndpointProvider) =>
{
    Order.API.Models.Order order = new Order.API.Models.Order()
    {
        BuyerId = model.BuyerId,
        OrderItems = model.OrderItems.Select(c => new OrderItem
        {
            Count = c.Count,
            ProductId = c.ProductId,
            Price = c.Price
        }).ToList(),
        OrderStatus = Order.API.Enums.OrderStatus.Suspend,
        CreateDate = DateTime.UtcNow,
        TotalPrice = model.OrderItems.Sum(c => c.Price * c.Count)
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderStartedEvent orderStartedEvent = new OrderStartedEvent()
    {
        BuyerId = model.BuyerId,
        OrderId = order.Id,
        TotalPrice = model.OrderItems.Sum(c => c.Count * c.Price),
        OrderItems = model.OrderItems.Select(c => new Shared.Messages.OrderItemMessage
        {
            Count = c.Count,
            Price = c.Price,
            ProductId = c.ProductId
        }).ToList()
    };

    ISendEndpoint sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue: {RabbitMQSettings.StateMachineQueue}"));
    await sendEndpoint.Send<OrderStartedEvent>(orderStartedEvent);
});

app.Run();

