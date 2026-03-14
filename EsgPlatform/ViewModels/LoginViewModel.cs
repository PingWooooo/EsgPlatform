using System.ComponentModel.DataAnnotations;

namespace EsgPlatform.ViewModels;

/// <summary>登入頁面資料模型</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "請輸入帳號")]
    [Display(Name = "帳號")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "請輸入密碼")]
    [DataType(DataType.Password)]
    [Display(Name = "密碼")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "記住我")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
