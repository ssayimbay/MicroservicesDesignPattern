namespace SagaChreography.Shared.Events
{
    public class StockNotReserverdEvent
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}
