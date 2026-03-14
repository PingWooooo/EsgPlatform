using EsgPlatform.Models;

namespace EsgPlatform.Repositories.Interfaces;

/// <summary>報告排程資料存取介面</summary>
public interface IReportScheduleRepository
{
    Task<IEnumerable<ReportSchedule>> GetAllAsync();
    Task<ReportSchedule?> GetByIdAsync(int id);
    Task<int> InsertAsync(ReportSchedule schedule);
    Task UpdateAsync(ReportSchedule schedule);
    Task DeleteAsync(int id);
}
