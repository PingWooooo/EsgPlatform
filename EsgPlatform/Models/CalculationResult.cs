namespace EsgPlatform.Models;

/// <summary>
/// CO2e 計算結果 - 對應 CalculationResults 資料表
/// </summary>
public class CalculationResult
{
    public int Id { get; set; }

    /// <summary>關聯的原始數據上傳 ID</summary>
    public int UploadId { get; set; }

    /// <summary>計算所得 CO2e 總量（公噸）</summary>
    public decimal TotalCO2e { get; set; }

    /// <summary>計算時間</summary>
    public DateTime CalculatedAt { get; set; }

    // --- JOIN 查詢擴充欄位 ---
    public string? Category { get; set; }
    public string? ItemName { get; set; }
    public int Scope { get; set; }
    public DateTime UploadDate { get; set; }
    public string? Username { get; set; }
}
