using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>報告燈號狀態資料存取介面</summary>
public interface IReportStatusLogRepository
{
    Task<IEnumerable<ReportStatusLog>> GetAllWithScheduleInfoAsync();
    Task<int> InsertAsync(ReportStatusLog log);
    Task UpdateStatusAsync(int scheduleId, string status, DateTime? lastUpdateDate);
    Task<ReportStatusLog?> GetLatestByScheduleIdAsync(int scheduleId);
}
