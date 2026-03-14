using EsgPlatform.Models;

namespace EsgPlatform.Services.Interfaces;

/// <summary>身份認證服務介面</summary>
public interface IAuthService
{
    /// <summary>驗證帳號密碼，回傳使用者物件；驗證失敗回傳 null</summary>
    Task<User?> ValidateUserAsync(string username, string password);

    /// <summary>驗證密碼雜湊是否匹配</summary>
    bool VerifyPassword(string password, string hash);

    /// <summary>產生 BCrypt 密碼雜湊</summary>
    string HashPassword(string password);
}
