using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>碳排係數設定存取介面</summary>
public interface IEmissionConfigRepository
{
    Task<IEnumerable<EmissionConfig>> GetAllAsync();
    Task<EmissionConfig?> GetByIdAsync(int id);
    Task<EmissionConfig?> GetByScopeAndItemAsync(int scope, string category, string itemName);
    Task<int> InsertAsync(EmissionConfig config);
    Task UpdateAsync(EmissionConfig config);
    Task DeleteAsync(int id);
}
