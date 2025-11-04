using BookStore.Models;

namespace BookStore.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category> 
{
    void update(Category obj);
}