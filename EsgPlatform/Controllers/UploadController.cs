using EsgPlatform.Services.Interfaces;
using EsgPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EsgPlatform.Controllers;

/// <summary>Excel 數據上傳控制器</summary>
[Authorize]
public class UploadController : Controller
{
    private readonly IUploadService _uploadService;
    private readonly ILogger<UploadController> _logger;

    // 允許的副檔名
    private static readonly string[] AllowedExtensions = [".xlsx", ".xls", ".csv"];
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public UploadController(IUploadService uploadService, ILogger<UploadController> logger)
    {
        _uploadService = uploadService;
        _logger        = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new UploadViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        var vm = new UploadViewModel { IsSubmitted = true };

        if (file == null || file.Length == 0)
        {
            vm.Message = "請選擇要上傳的 Excel 檔案";
            return View("Index", vm);
        }

        // 驗證檔案大小
        if (file.Length > MaxFileSizeBytes)
        {
            vm.Message = $"檔案大小超過限制（最大 10 MB），目前檔案：{file.Length / 1024 / 1024:F1} MB";
            return View("Index", vm);
        }

        // 驗證副檔名
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            vm.Message = $"不支援的檔案格式，請上傳 .xlsx 或 .csv 格式";
            return View("Index", vm);
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (userId == 0)
                return Challenge();

            using var stream = file.OpenReadStream();
            var (success, error, errors) = await _uploadService.ProcessExcelUploadAsync(stream, userId);

            vm.SuccessCount = success;
            vm.ErrorCount   = error;
            vm.Errors       = errors;
            vm.Message      = success > 0
                ? $"成功匯入 {success} 筆數據，並已完成 CO2e 計算"
                : "沒有成功匯入任何資料，請確認 Excel 格式是否正確";

            _logger.LogInformation(
                "使用者 {UserId} 上傳 Excel，成功 {S} 筆，失敗 {E} 筆",
                userId, success, error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理 Excel 上傳時發生例外");
            vm.Message = $"上傳處理失敗：{ex.Message}";
            vm.ErrorCount = 1;
        }

        return View("Index", vm);
    }

    /// <summary>下載 Excel 範本</summary>
    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "upload_template.xlsx");
        if (!System.IO.File.Exists(templatePath))
        {
            TempData["Error"] = "範本檔案不存在，請聯絡系統管理員";
            return RedirectToAction("Index");
        }
        return PhysicalFile(templatePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ESG數據上傳範本.xlsx");
    }
}
