using System;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.StateMaps;

namespace SagaStateMachine.Service.StateDBContext
{
	public class OrderStateDBContext : SagaDbContext
	{
        public OrderStateDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new OrderStateMap();
            }
        }
    }
}

