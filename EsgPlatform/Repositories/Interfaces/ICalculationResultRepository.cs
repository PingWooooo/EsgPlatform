using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>計算結果資料存取介面</summary>
public interface ICalculationResultRepository
{
    Task<int> InsertAsync(CalculationResult result);
    Task<IEnumerable<CalculationResult>> GetAllWithDetailsAsync();
    Task<IEnumerable<CalculationResult>> GetRecentAsync(int count = 20);
}
