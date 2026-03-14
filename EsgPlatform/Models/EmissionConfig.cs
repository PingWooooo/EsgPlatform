namespace EsgPlatform.Models;

/// <summary>
/// 碳排放係數設定 - 對應 EmissionConfigs 資料表
/// </summary>
public class EmissionConfig
{
    public int Id { get; set; }

    /// <summary>排放範疇：1 或 2</summary>
    public int Scope { get; set; }

    /// <summary>排放類別</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>排放項目名稱</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>排放係數值</summary>
    public decimal Factor { get; set; }

    /// <summary>全球暖化潛勢（GWP）</summary>
    public decimal GWP { get; set; } = 1;

    /// <summary>係數單位（例：kg CO2e/kWh）</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>最後更新時間</summary>
    public DateTime UpdatedAt { get; set; }
}
