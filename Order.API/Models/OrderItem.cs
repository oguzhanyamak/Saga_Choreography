namespace Order.API.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public long Price { get; set; }
        public uint Count { get; set; }
        public Order Order { get; set; }
    }
}