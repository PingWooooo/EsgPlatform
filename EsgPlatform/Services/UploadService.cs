using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.Services.Interfaces;
using MiniExcelLibs;

namespace EsgPlatform.Services;

/// <summary>
/// Excel 上傳與批次處理服務
/// 支援的 Excel 欄位：Scope, Category, ItemName, Value, Unit
/// </summary>
public class UploadService : IUploadService
{
    private readonly IRawDataRepository _rawDataRepo;
    private readonly ICalculationEngine _calculationEngine;
    private readonly ILogger<UploadService> _logger;

    public UploadService(
        IRawDataRepository rawDataRepo,
        ICalculationEngine calculationEngine,
        ILogger<UploadService> logger)
    {
        _rawDataRepo        = rawDataRepo;
        _calculationEngine  = calculationEngine;
        _logger             = logger;
    }

    public async Task<(int SuccessCount, int ErrorCount, List<string> Errors)> ProcessExcelUploadAsync(
        Stream fileStream, int userId)
    {
        var errors       = new List<string>();
        int successCount = 0;
        int errorCount   = 0;
        int rowIndex     = 1;

        // 寫入暫存檔以使用 MiniExcel 檔案路徑 API
        var tempPath = Path.Combine(Path.GetTempPath(), $"esg_upload_{Guid.NewGuid()}.xlsx");
        try
        {
            // 將 Stream 存為暫存檔，再以路徑呼叫 MiniExcel
            using (var fs = File.Create(tempPath))
                await fileStream.CopyToAsync(fs);

            // 使用 MiniExcel 解析 Excel（第一列為標題列）
            var rows = MiniExcel.Query<ExcelRowModel>(tempPath);

            foreach (var row in rows)
            {
                rowIndex++;
                try
                {
                    // 驗證必填欄位
                    if (row.Scope == 0 || string.IsNullOrWhiteSpace(row.Category) ||
                        string.IsNullOrWhiteSpace(row.ItemName) || row.Value <= 0)
                    {
                        errors.Add($"第 {rowIndex} 列：資料不完整或數值無效，已跳過");
                        errorCount++;
                        continue;
                    }

                    if (row.Scope != 1 && row.Scope != 2)
                    {
                        errors.Add($"第 {rowIndex} 列：範疇值 {row.Scope} 無效（僅接受 1 或 2）");
                        errorCount++;
                        continue;
                    }

                    var upload = new RawDataUpload
                    {
                        UserId     = userId,
                        Scope      = row.Scope,
                        Category   = row.Category.Trim(),
                        ItemName   = row.ItemName.Trim(),
                        Value      = row.Value,
                        Unit       = row.Unit?.Trim() ?? "未知",
                        UploadDate = DateTime.Now
                    };

                    // 儲存原始資料
                    upload.Id = await _rawDataRepo.InsertAsync(upload);

                    // 觸發計算引擎
                    await _calculationEngine.CalculateAndSaveAsync(upload);

                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "處理第 {Row} 列時發生錯誤", rowIndex);
                    errors.Add($"第 {rowIndex} 列：處理失敗 - {ex.Message}");
                    errorCount++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析 Excel 檔案時發生錯誤");
            errors.Add($"Excel 解析失敗：{ex.Message}");
            errorCount++;
        }
        finally
        {
            // 清除暫存檔
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }

        _logger.LogInformation(
            "Excel 上傳處理完成：成功 {Success} 筆，失敗 {Error} 筆",
            successCount, errorCount);

        return (successCount, errorCount, errors);
    }

    /// <summary>Excel 列對應模型（欄位名稱需與 Excel 標題列一致）</summary>
    private class ExcelRowModel
    {
        public int    Scope    { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public decimal Value   { get; set; }
        public string? Unit    { get; set; }
    }
}
