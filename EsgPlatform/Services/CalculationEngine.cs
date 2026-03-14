using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.Services.Interfaces;

namespace EsgPlatform.Services;

/// <summary>
/// CO2e 計算引擎
/// 公式：CO2e（kg）= 活動量 × 排放係數 × GWP
/// 最終儲存單位：公噸（除以 1000）
/// </summary>
public class CalculationEngine : ICalculationEngine
{
    private readonly IEmissionConfigRepository _configRepo;
    private readonly ICalculationResultRepository _resultRepo;
    private readonly ILogger<CalculationEngine> _logger;

    public CalculationEngine(
        IEmissionConfigRepository configRepo,
        ICalculationResultRepository resultRepo,
        ILogger<CalculationEngine> logger)
    {
        _configRepo = configRepo;
        _resultRepo = resultRepo;
        _logger = logger;
    }

    public async Task<decimal> CalculateAndSaveAsync(RawDataUpload upload)
    {
        // 查找對應的排放係數
        var config = await _configRepo.GetByScopeAndItemAsync(
            upload.Scope, upload.Category, upload.ItemName);

        if (config == null)
        {
            _logger.LogWarning(
                "找不到對應排放係數：範疇 {Scope}，類別 {Category}，項目 {ItemName}，跳過計算",
                upload.Scope, upload.Category, upload.ItemName);
            return 0;
        }

        // 計算 CO2e（公噸）
        var co2eKg = upload.Value * config.Factor * config.GWP;
        var co2eTonne = co2eKg / 1000m;

        var result = new CalculationResult
        {
            UploadId     = upload.Id,
            TotalCO2e    = co2eTonne,
            CalculatedAt = DateTime.Now
        };

        await _resultRepo.InsertAsync(result);

        _logger.LogInformation(
            "計算完成：UploadId={UploadId}，CO2e={CO2e:F4} 公噸",
            upload.Id, co2eTonne);

        return co2eTonne;
    }

    public async Task<IEnumerable<CalculationResult>> BatchCalculateAsync(IEnumerable<RawDataUpload> uploads)
    {
        var results = new List<CalculationResult>();

        foreach (var upload in uploads)
        {
            var config = await _configRepo.GetByScopeAndItemAsync(
                upload.Scope, upload.Category, upload.ItemName);

            if (config == null)
            {
                _logger.LogWarning(
                    "批次計算：找不到係數 Scope={Scope}, Item={Item}，已跳過",
                    upload.Scope, upload.ItemName);
                continue;
            }

            var co2eTonne = (upload.Value * config.Factor * config.GWP) / 1000m;

            var result = new CalculationResult
            {
                UploadId     = upload.Id,
                TotalCO2e    = co2eTonne,
                CalculatedAt = DateTime.Now
            };

            await _resultRepo.InsertAsync(result);
            results.Add(result);
        }

        _logger.LogInformation("批次計算完成，共 {Count} 筆", results.Count);
        return results;
    }
}
