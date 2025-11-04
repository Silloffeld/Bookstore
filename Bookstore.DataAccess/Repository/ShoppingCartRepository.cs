using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private ApplicationDbContext _context;

    public ShoppingCartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public int DecrementCount(ShoppingCart cart, int count)
    {
        cart.Count -= count;
        return cart.Count;
    }

    public int IncrementCount(ShoppingCart cart, int count)
    {
        cart.Count += count;
        return cart.Count;
    }

    public void Update(ShoppingCart obj)
    {
        _context.ShoppingCarts.Update(obj);
    }
}
