namespace EsgPlatform.Models;

/// <summary>
/// 角色資料模型 - 對應 Roles 資料表
/// </summary>
public class Role
{
    public int Id { get; set; }

    /// <summary>角色名稱：Admin 或 User</summary>
    public string RoleName { get; set; } = string.Empty;
}
