using MassTransit;
using Payment.API.Consumer;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<PaymentStartedEventConsumer>();
    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue,
     e => e.ConfigureConsumer<PaymentStartedEventConsumer>(context));
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();



app.Run();

