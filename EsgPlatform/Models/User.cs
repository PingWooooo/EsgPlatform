namespace EsgPlatform.Models;

/// <summary>
/// 會員主資料模型 - 對應 Users 資料表
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>使用者帳號名稱</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt 密碼雜湊值</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>電子郵件地址</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>角色 ID（關聯 Roles 資料表）</summary>
    public int RoleId { get; set; }

    /// <summary>建立時間</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>關聯角色物件（非資料庫欄位，JOIN 查詢用）</summary>
    public string? RoleName { get; set; }
}
