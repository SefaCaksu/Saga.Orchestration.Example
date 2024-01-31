using MassTransit;
using Shared.Messages;

namespace Shared.StockEvents
{
    public class StockReservedEvent : CorrelatedBy<Guid>
    {
        public StockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}

