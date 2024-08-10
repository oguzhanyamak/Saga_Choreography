namespace Order.API.VM
{
    public class CreateOrderVM
    {
        public string BuyerId { get; set; }
        public List<CreateOrderItemVM> OrderItems { get; set; }
    }

    public class CreateOrderItemVM
    {
        public string ProductId { get; set; }
        public uint Count { get; set; }
        public long Price { get; set; }
    }
}
