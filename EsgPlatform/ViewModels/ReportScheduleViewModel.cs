using System.ComponentModel.DataAnnotations;

namespace EsgPlatform.ViewModels;

/// <summary>報告排程新增/編輯頁面資料模型</summary>
public class ReportScheduleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "請輸入報告名稱")]
    [Display(Name = "報告名稱")]
    [StringLength(200)]
    public string ReportName { get; set; } = string.Empty;

    [Required(ErrorMessage = "請選擇頻率")]
    [Display(Name = "頻率")]
    public string Frequency { get; set; } = "Monthly";

    [Required(ErrorMessage = "請輸入負責窗口")]
    [Display(Name = "負責窗口")]
    [StringLength(100)]
    public string ResponsiblePerson { get; set; } = string.Empty;

    [Required(ErrorMessage = "請輸入警示天數")]
    [Display(Name = "警示天數")]
    [Range(1, 365, ErrorMessage = "警示天數必須介於 1 至 365 天")]
    public int WarningDays { get; set; } = 7;

    [Required(ErrorMessage = "請選擇下次截止日")]
    [Display(Name = "下次截止日")]
    [DataType(DataType.Date)]
    public DateOnly NextDueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
}
