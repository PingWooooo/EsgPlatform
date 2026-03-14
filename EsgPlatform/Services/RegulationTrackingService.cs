using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;

namespace EsgPlatform.Services;

/// <summary>
/// 法規係數變更追蹤服務
/// 當 EmissionConfig 被修改時，自動記錄變更歷程到 RegulationUpdateLogs
/// </summary>
public class RegulationTrackingService
{
    private readonly IEmissionConfigRepository _configRepo;
    private readonly IRegulationUpdateLogRepository _logRepo;
    private readonly ILogger<RegulationTrackingService> _logger;

    public RegulationTrackingService(
        IEmissionConfigRepository configRepo,
        IRegulationUpdateLogRepository logRepo,
        ILogger<RegulationTrackingService> logger)
    {
        _configRepo = configRepo;
        _logRepo    = logRepo;
        _logger     = logger;
    }

    /// <summary>
    /// 更新排放係數並自動記錄變更紀錄
    /// </summary>
    public async Task UpdateFactorWithTrackingAsync(
        int configId, decimal newFactor, string? changeReason = null)
    {
        var config = await _configRepo.GetByIdAsync(configId)
            ?? throw new InvalidOperationException($"找不到 ID={configId} 的排放係數設定");

        var oldFactor = config.Factor;

        // 若新舊值相同，無需更新
        if (oldFactor == newFactor)
        {
            _logger.LogInformation("排放係數未變更（ID={Id}），跳過更新", configId);
            return;
        }

        // 更新係數
        config.Factor = newFactor;
        await _configRepo.UpdateAsync(config);

        // 自動記錄變更歷程
        var log = new RegulationUpdateLog
        {
            ConfigId     = configId,
            OldValue     = oldFactor,
            NewValue     = newFactor,
            ChangeReason = changeReason,
            UpdateDate   = DateTime.Now
        };
        await _logRepo.InsertAsync(log);

        _logger.LogInformation(
            "法規係數更新：ConfigId={Id}，{Old} → {New}，原因：{Reason}",
            configId, oldFactor, newFactor, changeReason ?? "未填寫");
    }
}
