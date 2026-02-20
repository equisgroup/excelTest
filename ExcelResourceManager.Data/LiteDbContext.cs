using LiteDB;

namespace ExcelResourceManager.Data;

public class LiteDbContext : IDisposable
{
    private readonly LiteDatabase _database;
    
    public LiteDbContext(string connectionString)
    {
        _database = new LiteDatabase(connectionString);
    }
    
    public ILiteCollection<T> GetCollection<T>(string? name = null)
    {
        return _database.GetCollection<T>(name);
    }
    
    public void Dispose()
    {
        _database?.Dispose();
    }
}
