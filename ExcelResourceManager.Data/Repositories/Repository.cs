using System.Linq.Expressions;
using LiteDB;

namespace ExcelResourceManager.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly LiteDbContext _context;
    private readonly ILiteCollection<T> _collection;
    
    public Repository(LiteDbContext context)
    {
        _context = context;
        _collection = _context.GetCollection<T>();
    }
    
    public Task<T?> GetByIdAsync(int id)
    {
        return Task.FromResult(_collection.FindById(new BsonValue(id)));
    }
    
    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult(_collection.FindAll().AsEnumerable());
    }
    
    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult(_collection.Find(predicate).AsEnumerable());
    }
    
    public Task<int> InsertAsync(T entity)
    {
        var result = _collection.Insert(entity);
        return Task.FromResult(result.AsInt32);
    }
    
    public Task<bool> UpdateAsync(T entity)
    {
        return Task.FromResult(_collection.Update(entity));
    }
    
    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_collection.Delete(new BsonValue(id)));
    }
}
