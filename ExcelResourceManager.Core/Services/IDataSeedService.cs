namespace ExcelResourceManager.Core.Services;

public interface IDataSeedService
{
    Task SeedTestDataAsync();
    Task SeedProdDataAsync();
}
