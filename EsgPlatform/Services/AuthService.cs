using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.Services.Interfaces;

namespace EsgPlatform.Services;

/// <summary>身份認證服務實作（使用 BCrypt 驗證密碼）</summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            _logger.LogWarning("查無帳號：{Username}", username);
            return null;
        }
        if (!VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning("密碼驗證失敗：{Username}", username);
            return null;
        }
        return user;
    }

    public bool VerifyPassword(string password, string hash)
    {     

        // 使用內建方式驗證 BCrypt 雜湊
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }
}
