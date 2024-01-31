

using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.StateDBContext;
using SagaStateMachine.Service.StateInstance;
using SagaStateMachine.Service.StateMachines;
using Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
    .EntityFrameworkRepository(options=>
    {
        options.AddDbContext<DbContext, OrderStateDBContext>((provider, _builder) =>
        {
             _builder.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
        });
    });

    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);

        configurator.ReceiveEndpoint(RabbitMQSettings.StateMachineQueue, c => c.ConfigureSaga<OrderStateInstance>(context));
    });
});


var host = builder.Build();
host.Run();

