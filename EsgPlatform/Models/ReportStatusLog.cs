namespace EsgPlatform.Models;

/// <summary>
/// 報告進度燈號狀態 - 對應 ReportStatusLogs 資料表
/// </summary>
public class ReportStatusLog
{
    public int Id { get; set; }

    /// <summary>關聯排程 ID</summary>
    public int ScheduleId { get; set; }

    /// <summary>最後一次上傳資料時間（null 表示尚未上傳）</summary>
    public DateTime? LastUpdateDate { get; set; }

    /// <summary>本次截止日期</summary>
    public DateOnly NextDueDate { get; set; }

    /// <summary>燈號狀態：Green / Yellow / Red</summary>
    public string Status { get; set; } = "Red";

    // --- JOIN 查詢擴充欄位 ---
    public string? ReportName { get; set; }
    public string? Frequency { get; set; }
    public string? ResponsiblePerson { get; set; }

    /// <summary>燈號中文說明</summary>
    public string StatusDisplayName => Status switch
    {
        "Green"  => "正常",
        "Yellow" => "警告",
        "Red"    => "逾期",
        _        => "未知"
    };

    /// <summary>燈號 CSS 顏色 class</summary>
    public string StatusCssClass => Status switch
    {
        "Green"  => "status-green",
        "Yellow" => "status-yellow",
        "Red"    => "status-red",
        _        => ""
    };
}
