using System;
using MassTransit;
using Shared;
using Shared.PaymentEvents;

namespace Payment.API.Consumer
{
    public class PaymentStartedEventConsumer : IConsumer<PaymentStartedEvent>
    {
        readonly ISendEndpointProvider _sendEndpointProvider;

        public PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
        {
            var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

            if (true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId) { };
                sendEndPoint.Send(paymentCompletedEvent);
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Bakiye yetersiz.",
                    OrderItems = context.Message.OrderItems
                };
                sendEndPoint.Send(paymentFailedEvent);
            }
        }
    }
}

