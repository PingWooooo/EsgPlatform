using EsgPlatform.Models;

namespace EsgPlatform.Services.Interfaces;

/// <summary>Excel 上傳與資料處理服務介面</summary>
public interface IUploadService
{
    /// <summary>解析 Excel 並批次儲存原始數據，觸發計算引擎</summary>
    Task<(int SuccessCount, int ErrorCount, List<string> Errors)> ProcessExcelUploadAsync(
        Stream fileStream, int userId);
}
