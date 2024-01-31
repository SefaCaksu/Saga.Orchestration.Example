using MassTransit;
using Shared.Messages;

namespace Shared.PaymentEvents
{
    public class PaymentCompletedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }

        public PaymentCompletedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
	}
}

