using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.Services.Interfaces;
using EsgPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsgPlatform.Controllers;

/// <summary>儀表板控制器</summary>
[Authorize]
public class DashboardController : Controller
{
    private readonly IRawDataRepository _rawDataRepo;
    private readonly ICalculationResultRepository _resultRepo;
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IRawDataRepository rawDataRepo,
        ICalculationResultRepository resultRepo,
        IMonitoringService monitoringService,
        ILogger<DashboardController> logger)
    {
        _rawDataRepo       = rawDataRepo;
        _resultRepo        = resultRepo;
        _monitoringService = monitoringService;
        _logger            = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var scopeSummary  = await _rawDataRepo.GetScopeCO2eSummaryAsync();
            var recentResults = await _resultRepo.GetRecentAsync(10);
            var monitoring    = await _monitoringService.GetMonitoringListAsync();

            var vm = new DashboardViewModel
            {
                Scope1Total   = scopeSummary.TryGetValue(1, out var s1) ? s1 : 0,
                Scope2Total   = scopeSummary.TryGetValue(2, out var s2) ? s2 : 0,
                MonitoringList = monitoring,
                RecentResults  = recentResults
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "載入儀表板時發生錯誤");
            TempData["Error"] = "載入儀表板資料時發生錯誤，請稍後再試";
            return View(new DashboardViewModel());
        }
    }
}
