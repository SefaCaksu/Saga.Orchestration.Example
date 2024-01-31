using MassTransit;

namespace Shared.StockEvents
{
    public class StockNotReservedEvent : CorrelatedBy<Guid>
    {
        public StockNotReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
        public string Message { get; set; }
    }
}

