using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Product obj)
    {
        var objFromDb = _context.Products.FirstOrDefault(p => p.Id == obj.Id);
        if (objFromDb != null)
        {
            objFromDb.Title = obj.Title;
            objFromDb.ISBN = obj.ISBN;
            objFromDb.Author = obj.Author;
            objFromDb.Description = obj.Description;
            objFromDb.ListPrice = obj.ListPrice;
            objFromDb.Price = obj.Price;
            objFromDb.Price50 = obj.Price50;
            objFromDb.Price100 = obj.Price100;
            objFromDb.CategoryId = obj.CategoryId;
            objFromDb.CoverTypeId = obj.CoverTypeId;
            
            // Only update ImageUrl if a new one is provided
            if (!string.IsNullOrEmpty(obj.ImageUrl))
            {
                objFromDb.ImageUrl = obj.ImageUrl;
            }
        }
    }
}
