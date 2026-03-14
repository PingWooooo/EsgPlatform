using EsgPlatform.Models;

namespace EsgPlatform.Services.Interfaces;

/// <summary>紅綠燈監控服務介面</summary>
public interface IMonitoringService
{
    /// <summary>重新計算所有排程的燈號狀態並更新資料庫</summary>
    Task RefreshAllStatusAsync();

    /// <summary>依照規則計算單一排程的燈號</summary>
    string CalculateStatus(ReportSchedule schedule, DateTime? lastUploadDate);

    /// <summary>取得最新監控列表（含燈號）</summary>
    Task<IEnumerable<ReportStatusLog>> GetMonitoringListAsync();
}
