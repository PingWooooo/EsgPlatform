using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>法規更新紀錄資料存取介面</summary>
public interface IRegulationUpdateLogRepository
{
    Task<int> InsertAsync(RegulationUpdateLog log);
    Task<IEnumerable<RegulationUpdateLog>> GetAllWithDetailsAsync();
}
