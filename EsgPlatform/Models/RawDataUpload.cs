namespace EsgPlatform.Models;

/// <summary>
/// 原始排放數據上傳記錄 - 對應 RawDataUploads 資料表
/// </summary>
public class RawDataUpload
{
    public int Id { get; set; }

    /// <summary>上傳者 ID（關聯 Users）</summary>
    public int UserId { get; set; }

    /// <summary>排放範疇：1 或 2</summary>
    public int Scope { get; set; }

    /// <summary>排放類別（例：固定燃燒源）</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>排放項目名稱（例：天然氣）</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>活動量數值</summary>
    public decimal Value { get; set; }

    /// <summary>單位（例：m³、kWh）</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>上傳時間</summary>
    public DateTime UploadDate { get; set; }

    /// <summary>上傳者名稱（JOIN 查詢用）</summary>
    public string? Username { get; set; }
}
