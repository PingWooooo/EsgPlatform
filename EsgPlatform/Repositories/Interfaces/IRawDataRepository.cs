using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>原始排放數據存取介面</summary>
public interface IRawDataRepository
{
    Task<int> InsertAsync(RawDataUpload data);
    Task<IEnumerable<RawDataUpload>> GetRecentAsync(int count = 50);
    Task<IEnumerable<RawDataUpload>> GetByScopeAsync(int scope);
    Task<IEnumerable<RawDataUpload>> GetAllAsync();

    /// <summary>取得各範疇的 CO2e 加總（用於圓餅圖）</summary>
    Task<Dictionary<int, decimal>> GetScopeCO2eSummaryAsync();
}
