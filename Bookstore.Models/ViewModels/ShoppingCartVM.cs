namespace BookStore.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartList { get; set; } = new List<ShoppingCart>();
        public OrderHeader OrderHeader { get; set; } = new OrderHeader();
    }
}
