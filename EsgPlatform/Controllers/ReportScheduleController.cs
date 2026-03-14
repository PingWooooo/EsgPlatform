using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsgPlatform.Controllers;

/// <summary>報告排程維護控制器（僅限 Admin）</summary>
[Authorize(Roles = "Admin")]
public class ReportScheduleController : Controller
{
    private readonly IReportScheduleRepository _scheduleRepo;
    private readonly ILogger<ReportScheduleController> _logger;

    public ReportScheduleController(
        IReportScheduleRepository scheduleRepo,
        ILogger<ReportScheduleController> logger)
    {
        _scheduleRepo = scheduleRepo;
        _logger       = logger;
    }

    /// <summary>排程清單</summary>
    public async Task<IActionResult> Index()
    {
        var schedules = await _scheduleRepo.GetAllAsync();
        return View(schedules);
    }

    /// <summary>新增排程頁</summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new ReportScheduleViewModel
        {
            WarningDays  = 7,
            Frequency    = "Monthly",
            NextDueDate  = DateOnly.FromDateTime(DateTime.Today.AddMonths(1))
        });
    }

    /// <summary>新增排程處理</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReportScheduleViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var schedule = new ReportSchedule
            {
                ReportName        = vm.ReportName,
                Frequency         = vm.Frequency,
                ResponsiblePerson = vm.ResponsiblePerson,
                WarningDays       = vm.WarningDays,
                NextDueDate       = vm.NextDueDate
            };

            await _scheduleRepo.InsertAsync(schedule);
            TempData["Success"] = $"排程「{schedule.ReportName}」建立成功";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增排程時發生錯誤");
            ModelState.AddModelError(string.Empty, "新增失敗，請稍後再試");
            return View(vm);
        }
    }

    /// <summary>編輯排程頁</summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var schedule = await _scheduleRepo.GetByIdAsync(id);
        if (schedule == null)
            return NotFound();

        var vm = new ReportScheduleViewModel
        {
            Id                = schedule.Id,
            ReportName        = schedule.ReportName,
            Frequency         = schedule.Frequency,
            ResponsiblePerson = schedule.ResponsiblePerson,
            WarningDays       = schedule.WarningDays,
            NextDueDate       = schedule.NextDueDate
        };
        return View(vm);
    }

    /// <summary>編輯排程處理</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ReportScheduleViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var schedule = new ReportSchedule
            {
                Id                = vm.Id,
                ReportName        = vm.ReportName,
                Frequency         = vm.Frequency,
                ResponsiblePerson = vm.ResponsiblePerson,
                WarningDays       = vm.WarningDays,
                NextDueDate       = vm.NextDueDate
            };

            await _scheduleRepo.UpdateAsync(schedule);
            TempData["Success"] = $"排程「{schedule.ReportName}」更新成功";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新排程 {Id} 時發生錯誤", vm.Id);
            ModelState.AddModelError(string.Empty, "更新失敗，請稍後再試");
            return View(vm);
        }
    }

    /// <summary>刪除排程確認頁</summary>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _scheduleRepo.GetByIdAsync(id);
        if (schedule == null)
            return NotFound();
        return View(schedule);
    }

    /// <summary>刪除排程處理</summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _scheduleRepo.DeleteAsync(id);
            TempData["Success"] = "排程刪除成功";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除排程 {Id} 時發生錯誤", id);
            TempData["Error"] = "刪除失敗，請稍後再試";
        }
        return RedirectToAction(nameof(Index));
    }
}
