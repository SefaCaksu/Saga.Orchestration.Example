using MassTransit;

namespace SagaStateMachine.Service.StateInstance
{
    public class OrderStateInstance : SagaStateMachineInstance
	{
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }
        public int BuyerId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

