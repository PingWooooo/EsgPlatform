using EsgPlatform.Models;

namespace EsgPlatform.Services.Interfaces;

/// <summary>CO2e 計算引擎服務介面</summary>
public interface ICalculationEngine
{
    /// <summary>
    /// 依據上傳資料查找對應係數並計算 CO2e，
    /// 計算完成後自動寫入 CalculationResults 資料表
    /// </summary>
    Task<decimal> CalculateAndSaveAsync(RawDataUpload upload);

    /// <summary>批次計算並儲存多筆上傳資料</summary>
    Task<IEnumerable<CalculationResult>> BatchCalculateAsync(IEnumerable<RawDataUpload> uploads);
}
