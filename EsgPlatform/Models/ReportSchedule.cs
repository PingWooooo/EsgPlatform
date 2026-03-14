namespace EsgPlatform.Models;

/// <summary>
/// 報告排程設定 - 對應 ReportSchedules 資料表
/// </summary>
public class ReportSchedule
{
    public int Id { get; set; }

    /// <summary>報告名稱</summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>頻率：Monthly（月報）或 Yearly（年報）</summary>
    public string Frequency { get; set; } = string.Empty;

    /// <summary>負責窗口人員</summary>
    public string ResponsiblePerson { get; set; } = string.Empty;

    /// <summary>提前警示天數（幾天前變黃燈）</summary>
    public int WarningDays { get; set; }

    /// <summary>下次截止日期</summary>
    public DateOnly NextDueDate { get; set; }

    /// <summary>建立時間</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>頻率中文顯示名稱（計算欄位）</summary>
    public string FrequencyDisplayName => Frequency == "Monthly" ? "月報" : "年報";
}
