# ESG 企業永續數據盤查平台

> 依 ISO 14064 / GHG Protocol 規範設計的溫室氣體排放盤查系統

## 技術架構

| 層次 | 技術 |
|------|------|
| 框架 | .NET 8.0 MVC（Repository + Service Pattern） |
| ORM | Dapper |
| 資料庫 | Microsoft SQL Server |
| 認證 | Cookie-based Authentication |
| Excel 處理 | MiniExcel |
| 前端 | Bootstrap 5.3 + Chart.js 4.4 |

## 系統功能

### 核心模組
- **儀表板**：Chart.js 圓餅圖顯示範疇一、二佔比 + 紅綠燈排程監控
- **Excel 數據上傳**：拖曳式上傳，自動觸發 CO₂e 計算引擎
- **報告排程維護**（Admin Only）：完整 CRUD 管理排程與負責窗口
- **資料匯出**：計算結果與原始數據匯出為 Excel

### 計算規則
- 公式：`CO₂e（kg）= 活動量 × 排放係數 × GWP`
- 儲存單位：公噸（kg ÷ 1000）

### 紅綠燈規則
| 頻率 | 綠燈 | 黃燈 | 紅燈 |
|------|------|------|------|
| 月報 | 已上傳且在截止日前 | 截止日 **7** 天前未上傳 | 已逾期 |
| 年報 | 已上傳且在截止日前 | 截止日 **90** 天前未上傳 | 已逾期 |

## 執行步驟

### 1. 資料庫初始化

連接 SQL Server 後執行以下腳本：

```sql
-- 使用 SQL Server Management Studio 或 sqlcmd 執行
sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -i SQL/InitialSchema.sql
```

或使用 SSMS / Azure Data Studio 開啟 `SQL/InitialSchema.sql` 後執行。

> **注意**：腳本會先刪除舊資料庫再重建，生產環境請移除前兩段 `IF EXISTS` 區塊。

### 2. 設定連線字串

編輯 `appsettings.json`，修改 `DefaultConnection` 中的 Server、User Id 與 Password：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EsgPlatform;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### 3. 啟動應用程式

```bash
cd EsgPlatform
dotnet run
```

瀏覽器開啟 `https://localhost:5001` 或 `http://localhost:5000`

### 4. 預設帳號

| 角色 | 帳號 | 密碼 |
|------|------|------|
| 管理員 | `admin` | `Admin@123` |
| 一般使用者 | `user01` | `User@123` |

## Excel 上傳格式

第一列為標題列，欄位名稱如下：

| 欄位名稱 | 說明 | 必填 |
|----------|------|------|
| `Scope` | 排放範疇（1 或 2） | 是 |
| `Category` | 排放類別（需與係數表一致） | 是 |
| `ItemName` | 排放項目名稱 | 是 |
| `Value` | 活動量數值 | 是 |
| `Unit` | 單位（如 m³、kWh） | 否 |

## 資料庫 Schema

```
Roles ──< Users ──< RawDataUploads ──< CalculationResults

EmissionConfigs ──< RegulationUpdateLogs

ReportSchedules ──< ReportStatusLogs
```

## 專案結構

```
EsgPlatform/
├── Controllers/         # MVC 控制器
├── Models/              # 資料模型（對應資料表）
├── ViewModels/          # 頁面資料傳遞模型
├── Repositories/        # 資料存取層（Dapper）
│   └── Interfaces/
├── Services/            # 業務邏輯層
│   └── Interfaces/
├── Views/               # Razor 頁面（繁體中文）
├── wwwroot/             # 靜態資源
├── SQL/
│   └── InitialSchema.sql  # 資料庫建立腳本
├── Program.cs           # 應用程式進入點 + DI 設定
└── appsettings.json     # 設定檔
```
