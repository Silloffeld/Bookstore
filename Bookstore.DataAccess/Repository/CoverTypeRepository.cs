using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository;

public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
{
    private ApplicationDbContext _context;

    public CoverTypeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void update(CoverType obj)
    {
        _context.CoverTypes.Update(obj);
    }
}
