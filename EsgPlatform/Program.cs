using EsgPlatform.Repositories;
using EsgPlatform.Repositories.Interfaces;
using EsgPlatform.Services;
using EsgPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 服務註冊
// ============================================================

builder.Services.AddControllersWithViews();

// Cookie 身份認證設定
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath        = "/Account/Login";
        options.LogoutPath       = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan   = TimeSpan.FromMinutes(
            builder.Configuration.GetValue<int>("AppSettings:CookieExpireMinutes", 480));
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly   = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Repository 依賴注入（Scoped：每次 HTTP 請求建立一個實例）
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRawDataRepository, RawDataRepository>();
builder.Services.AddScoped<IEmissionConfigRepository, EmissionConfigRepository>();
builder.Services.AddScoped<ICalculationResultRepository, CalculationResultRepository>();
builder.Services.AddScoped<IReportScheduleRepository, ReportScheduleRepository>();
builder.Services.AddScoped<IReportStatusLogRepository, ReportStatusLogRepository>();
builder.Services.AddScoped<IRegulationUpdateLogRepository, RegulationUpdateLogRepository>();

// Service 依賴注入
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICalculationEngine, CalculationEngine>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<RegulationTrackingService>();

// 上傳檔案大小限制（10 MB）
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// ============================================================
// 建置應用程式
// ============================================================

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 認證 / 授權中介軟體（順序不可顛倒）
app.UseAuthentication();
app.UseAuthorization();

// 預設路由：登入頁
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
