namespace EsgPlatform.ViewModels;

/// <summary>Excel 上傳頁面資料模型</summary>
public class UploadViewModel
{
    public bool IsSubmitted { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = [];
    public string? Message { get; set; }
}
