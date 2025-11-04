using BookStore.Models;

namespace BookStore.DataAccess.Repository.IRepository;

public interface ICoverTypeRepository : IRepository<CoverType> 
{
    void update(CoverType obj);
}
