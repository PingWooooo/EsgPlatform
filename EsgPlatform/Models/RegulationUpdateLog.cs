namespace EsgPlatform.Models;

/// <summary>
/// 法規係數變更追蹤紀錄 - 對應 RegulationUpdateLogs 資料表
/// </summary>
public class RegulationUpdateLog
{
    public int Id { get; set; }

    /// <summary>關聯係數設定 ID</summary>
    public int ConfigId { get; set; }

    /// <summary>修改前舊係數值</summary>
    public decimal OldValue { get; set; }

    /// <summary>修改後新係數值</summary>
    public decimal NewValue { get; set; }

    /// <summary>變更原因說明</summary>
    public string? ChangeReason { get; set; }

    /// <summary>更新時間</summary>
    public DateTime UpdateDate { get; set; }

    // --- JOIN 查詢擴充欄位 ---
    public string? ItemName { get; set; }
    public string? Category { get; set; }
    public int Scope { get; set; }
}
