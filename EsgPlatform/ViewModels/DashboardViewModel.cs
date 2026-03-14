using EsgPlatform.Models;

namespace EsgPlatform.ViewModels;

/// <summary>儀表板頁面資料模型</summary>
public class DashboardViewModel
{
    /// <summary>範疇一 CO2e 總量（公噸）</summary>
    public decimal Scope1Total { get; set; }

    /// <summary>範疇二 CO2e 總量（公噸）</summary>
    public decimal Scope2Total { get; set; }

    /// <summary>總 CO2e（公噸）</summary>
    public decimal GrandTotal => Scope1Total + Scope2Total;

    /// <summary>排程監控燈號清單</summary>
    public IEnumerable<ReportStatusLog> MonitoringList { get; set; } = [];

    /// <summary>最近上傳紀錄</summary>
    public IEnumerable<CalculationResult> RecentResults { get; set; } = [];

    /// <summary>紅燈數量</summary>
    public int RedCount => MonitoringList.Count(x => x.Status == "Red");

    /// <summary>黃燈數量</summary>
    public int YellowCount => MonitoringList.Count(x => x.Status == "Yellow");

    /// <summary>綠燈數量</summary>
    public int GreenCount => MonitoringList.Count(x => x.Status == "Green");
}
