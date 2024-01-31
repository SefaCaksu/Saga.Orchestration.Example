using MassTransit;
using SagaStateMachine.Service.StateInstance;
using Shared;
using Shared.Messages;
using Shared.OrderEvents;
using Shared.PaymentEvents;
using Shared.StockEvents;

namespace SagaStateMachine.Service.StateMachines
{
	public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
	{
		public Event<OrderStartedEvent> OrderStartedEvent { get; set; }
        public Event<StockReservedEvent> StockReservedEvent { get; set; }
        public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<StockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }

        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }
        public State StockNotReserved { get; set; }
        public State PaymentCompleted { get; set; }
        public State PaymentFailed { get; set; }


        public OrderStateMachine()
		{
			InstanceState(instance => instance.CurrentState);

			//Order id db de yoksa yeni correlateid oluştur
			Event(() => OrderStartedEvent,
				c => c.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId)
				.SelectId(e => Guid.NewGuid()));

			Event(() => StockReservedEvent,
				c => c.CorrelateById(@event => @event.Message.CorrelationId));

            Event(() => StockNotReservedEvent,
                c => c.CorrelateById(@event => @event.Message.CorrelationId));

            Event(() => PaymentCompletedEvent,
                c => c.CorrelateById(@event => @event.Message.CorrelationId));

            Event(() => PaymentFailedEvent,
              c => c.CorrelateById(@event => @event.Message.CorrelationId));


            //Tetikleyici event geldiğinde
            Initially(When(OrderStartedEvent)
                //işlem start edildiğinde db nesneleri loglanıyor
                .Then(context =>
                {
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.BuyerId = context.Data.BuyerId;
                    context.Instance.TotalPrice = context.Data.TotalPrice;
                    context.Instance.CreateDate = DateTime.UtcNow;
                })
                //transaction statusü belirleniyor
                .TransitionTo(OrderCreated)
                //Diğer servise işini yapması için quyryk fırlatılıyor.
                .Send(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEventQueue}"),
                    context => new OrderCreatedEvent(context.Instance.CorrelationId)
                    {
                        OrderItems = context.Data.OrderItems
                    })
                );

            //Tetikleyici Eventen dışındaki durumlar
            During(OrderCreated,
                When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue:{RabbitMQSettings.Payment_StartedEventQueue}"),
                context => new PaymentStartedEvent(context.Instance.CorrelationId)
                {
                    TotalPrice = context.Instance.TotalPrice,
                    OrderItems = context.Data.OrderItems
                }),
                When(StockNotReservedEvent)
                .TransitionTo(StockNotReserved)
                 .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"),
                    context => new OrderFailedEvent()
                    {
                        OrderId = context.Instance.OrderId,
                        Message = context.Data.Message
                    })
                
                ) ;

            During(StockReserved,
                When(PaymentCompletedEvent)
                .TransitionTo(PaymentCompleted)
                  .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderCompletedEventQueue}"),
                    context => new OrderCompletedEvent()
                    {
                        OrderId = context.Instance.OrderId,
                    })
                    .Finalize(),
                When(PaymentFailedEvent)
                .TransitionTo(PaymentFailed)
                .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"),
                    context => new OrderFailedEvent()
                    {
                        OrderId = context.Instance.OrderId,
                        Message = context.Data.Message
                    })
                 .Send(new Uri($"queue:{RabbitMQSettings.Stock_RollbackMessageQueue}"),
                    context => new StockRollBackMessage()
                    {
                        OrderItems = context.Data.OrderItems
                    })
                );

            SetCompletedWhenFinalized();
        }
	}
}

