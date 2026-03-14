using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.Services.Interfaces;

namespace EsgPlatform.Services;

/// <summary>
/// 紅綠燈監控服務
/// 燈號規則：
///   月報：截止日 7 天前未上傳 → 黃燈；已逾期 → 紅燈；已上傳且在截止日前 → 綠燈
///   年報：截止日 90 天前未上傳 → 黃燈；已逾期 → 紅燈；已上傳且在截止日前 → 綠燈
/// </summary>
public class MonitoringService : IMonitoringService
{
    private readonly IReportScheduleRepository _scheduleRepo;
    private readonly IReportStatusLogRepository _statusRepo;
    private readonly IRawDataRepository _rawDataRepo;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(
        IReportScheduleRepository scheduleRepo,
        IReportStatusLogRepository statusRepo,
        IRawDataRepository rawDataRepo,
        ILogger<MonitoringService> logger)
    {
        _scheduleRepo = scheduleRepo;
        _statusRepo   = statusRepo;
        _rawDataRepo  = rawDataRepo;
        _logger       = logger;
    }

    public async Task RefreshAllStatusAsync()
    {
        var schedules = await _scheduleRepo.GetAllAsync();
        var today     = DateOnly.FromDateTime(DateTime.Today);

        foreach (var schedule in schedules)
        {
            // 找最後一次有資料上傳的時間（以截止日前的最新一筆為準）
            var allUploads = await _rawDataRepo.GetAllAsync();
            var lastUpload = allUploads
                .Where(u => u.UploadDate.Date <= schedule.NextDueDate.ToDateTime(TimeOnly.MaxValue).Date)
                .OrderByDescending(u => u.UploadDate)
                .FirstOrDefault();

            var status = CalculateStatus(schedule, lastUpload?.UploadDate);

            var existing = await _statusRepo.GetLatestByScheduleIdAsync(schedule.Id);
            if (existing == null)
            {
                // 初次建立
                await _statusRepo.InsertAsync(new ReportStatusLog
                {
                    ScheduleId     = schedule.Id,
                    LastUpdateDate = lastUpload?.UploadDate,
                    NextDueDate    = schedule.NextDueDate,
                    Status         = status
                });
            }
            else
            {
                await _statusRepo.UpdateStatusAsync(schedule.Id, status, lastUpload?.UploadDate);
            }
        }

        _logger.LogInformation("排程監控燈號刷新完成，共處理 {Count} 筆排程", schedules.Count());
    }

    public string CalculateStatus(ReportSchedule schedule, DateTime? lastUploadDate)
    {
        var today   = DateTime.Today;
        var dueDate = schedule.NextDueDate.ToDateTime(TimeOnly.MinValue);

        // 已逾期（截止日已過且沒有有效上傳）
        if (today > dueDate)
        {
            if (lastUploadDate.HasValue && lastUploadDate.Value.Date <= dueDate.Date)
                return "Green"; // 已在期限內上傳
            return "Red";
        }

        // 尚在截止日前：判斷是否有上傳
        if (lastUploadDate.HasValue)
        {
            return "Green"; // 已上傳且在期限內
        }

        // 未上傳，計算距截止日天數
        var daysLeft = (dueDate - today).Days;

        return daysLeft <= schedule.WarningDays ? "Yellow" : "Green";
    }

    public async Task<IEnumerable<ReportStatusLog>> GetMonitoringListAsync()
    {
        // 每次取得前先刷新燈號
        await RefreshAllStatusAsync();
        return await _statusRepo.GetAllWithScheduleInfoAsync();
    }
}
