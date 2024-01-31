using MassTransit;
using Shared.Messages;

namespace Shared.PaymentEvents
{
    public class PaymentStartedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public PaymentStartedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}

