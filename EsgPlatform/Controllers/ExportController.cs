using EsgPlatform.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace EsgPlatform.Controllers;

/// <summary>資料匯出控制器</summary>
[Authorize]
public class ExportController : Controller
{
    private readonly ICalculationResultRepository _resultRepo;
    private readonly IRawDataRepository _rawDataRepo;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        ICalculationResultRepository resultRepo,
        IRawDataRepository rawDataRepo,
        ILogger<ExportController> logger)
    {
        _resultRepo   = resultRepo;
        _rawDataRepo  = rawDataRepo;
        _logger       = logger;
    }

    public async Task<IActionResult> Index()
    {
        var results = await _resultRepo.GetAllWithDetailsAsync();
        return View(results);
    }

    /// <summary>匯出計算結果為 Excel</summary>
    [HttpGet]
    public async Task<IActionResult> ExportResults()
    {
        try
        {
            var results = await _resultRepo.GetAllWithDetailsAsync();

            var exportData = results.Select(r => new
            {
                範疇         = $"範疇{r.Scope}",
                排放類別     = r.Category,
                排放項目     = r.ItemName,
                計算CO2e公噸  = r.TotalCO2e,
                上傳時間     = r.UploadDate.ToString("yyyy-MM-dd HH:mm"),
                計算時間     = r.CalculatedAt.ToString("yyyy-MM-dd HH:mm"),
                上傳者       = r.Username
            });

            var stream = new MemoryStream();
            await stream.SaveAsAsync(exportData);
            stream.Position = 0;

            var fileName = $"ESG計算結果_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            _logger.LogInformation("使用者 {User} 匯出計算結果", User.Identity?.Name);

            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出計算結果時發生錯誤");
            TempData["Error"] = $"匯出失敗：{ex.Message}";
            return RedirectToAction("Index");
        }
    }

    /// <summary>匯出原始上傳數據為 Excel</summary>
    [HttpGet]
    public async Task<IActionResult> ExportRawData()
    {
        try
        {
            var rawData = await _rawDataRepo.GetAllAsync();

            var exportData = rawData.Select(r => new
            {
                範疇     = $"範疇{r.Scope}",
                排放類別 = r.Category,
                排放項目 = r.ItemName,
                活動量   = r.Value,
                單位     = r.Unit,
                上傳時間 = r.UploadDate.ToString("yyyy-MM-dd HH:mm"),
                上傳者   = r.Username
            });

            var stream = new MemoryStream();
            await stream.SaveAsAsync(exportData);
            stream.Position = 0;

            var fileName = $"ESG原始數據_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "匯出原始數據時發生錯誤");
            TempData["Error"] = $"匯出失敗：{ex.Message}";
            return RedirectToAction("Index");
        }
    }
}
