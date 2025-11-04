namespace BookStore.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; } = new OrderHeader();
        public IEnumerable<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();
    }
}
