using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>使用者資料存取介面</summary>
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
}
