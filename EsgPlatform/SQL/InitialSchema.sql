-- ============================================================
-- ESG 企業永續數據盤查平台 - 資料庫初始化腳本
-- 資料庫：Microsoft SQL Server
-- 版本：1.0.0
-- 建立日期：2024
-- 說明：請依序執行本腳本，所有資料表將依照外鍵關聯順序建立
-- ============================================================

USE master;
GO

-- 若資料庫已存在則先卸離，重新建立（生產環境請移除此段）
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'EsgPlatform')
BEGIN
    ALTER DATABASE EsgPlatform SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE EsgPlatform;
END
GO

CREATE DATABASE EsgPlatform
    COLLATE Chinese_Taiwan_Stroke_CI_AS;
GO

USE EsgPlatform;
GO

-- ============================================================
-- 1. Roles 角色資料表（先建立，供 Users 參照）
-- ============================================================
CREATE TABLE Roles (
    Id          INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50)    NOT NULL UNIQUE  -- Admin / User
);
GO

-- 插入預設角色
INSERT INTO Roles (RoleName) VALUES (N'Admin');
INSERT INTO Roles (RoleName) VALUES (N'User');
GO

-- ============================================================
-- 2. Users 會員主資料表
-- ============================================================
CREATE TABLE Users (
    Id              INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Username        NVARCHAR(100)   NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(256)   NOT NULL,   -- BCrypt 雜湊
    Email           NVARCHAR(200)   NOT NULL UNIQUE,
    RoleId          INT             NOT NULL,
    CreatedAt       DATETIME2       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- 建立索引以加速查詢
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email    ON Users(Email);
GO

-- ============================================================
-- 3. EmissionConfigs 碳排係數資料表
-- ============================================================
CREATE TABLE EmissionConfigs (
    Id          INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Scope       INT             NOT NULL,           -- 1 或 2
    Category    NVARCHAR(100)   NOT NULL,           -- 例：固定燃燒、移動燃燒
    ItemName    NVARCHAR(200)   NOT NULL,           -- 例：天然氣、柴油
    Factor      DECIMAL(18,6)   NOT NULL,           -- 排放係數
    GWP         DECIMAL(10,4)   NOT NULL DEFAULT 1, -- 全球暖化潛勢
    Unit        NVARCHAR(50)    NOT NULL,           -- 例：kg CO2e / kWh
    UpdatedAt   DATETIME2       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_EmissionConfigs UNIQUE (Scope, Category, ItemName)
);
GO

-- ============================================================
-- 4. RawDataUploads 原始數據上傳資料表
-- ============================================================
CREATE TABLE RawDataUploads (
    Id          INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    UserId      INT             NOT NULL,
    Scope       INT             NOT NULL,           -- 1 或 2
    Category    NVARCHAR(100)   NOT NULL,
    ItemName    NVARCHAR(200)   NOT NULL,
    Value       DECIMAL(18,4)   NOT NULL,           -- 活動數據量
    Unit        NVARCHAR(50)    NOT NULL,
    UploadDate  DATETIME2       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RawDataUploads_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

CREATE INDEX IX_RawDataUploads_UserId     ON RawDataUploads(UserId);
CREATE INDEX IX_RawDataUploads_UploadDate ON RawDataUploads(UploadDate);
GO

-- ============================================================
-- 5. CalculationResults 計算結果資料表
-- ============================================================
CREATE TABLE CalculationResults (
    Id              INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    UploadId        INT             NOT NULL,
    TotalCO2e       DECIMAL(18,4)   NOT NULL,       -- 單位：公噸 CO2e
    CalculatedAt    DATETIME2       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_CalculationResults_Uploads FOREIGN KEY (UploadId) REFERENCES RawDataUploads(Id)
);
GO

CREATE INDEX IX_CalculationResults_UploadId ON CalculationResults(UploadId);
GO

-- ============================================================
-- 6. RegulationUpdateLogs 法規係數更新紀錄資料表
-- ============================================================
CREATE TABLE RegulationUpdateLogs (
    Id              INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ConfigId        INT             NOT NULL,
    OldValue        DECIMAL(18,6)   NOT NULL,       -- 舊排放係數
    NewValue        DECIMAL(18,6)   NOT NULL,       -- 新排放係數
    ChangeReason    NVARCHAR(500)   NULL,           -- 變更原因說明
    UpdateDate      DATETIME2       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RegulationUpdateLogs_Configs FOREIGN KEY (ConfigId) REFERENCES EmissionConfigs(Id)
);
GO

CREATE INDEX IX_RegulationUpdateLogs_ConfigId ON RegulationUpdateLogs(ConfigId);
GO

-- ============================================================
-- 7. ReportSchedules 報告排程資料表
-- ============================================================
CREATE TABLE ReportSchedules (
    Id                  INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ReportName          NVARCHAR(200)   NOT NULL,
    Frequency           NVARCHAR(20)    NOT NULL CHECK (Frequency IN (N'Monthly', N'Yearly')),
    ResponsiblePerson   NVARCHAR(100)   NOT NULL,   -- 負責窗口
    WarningDays         INT             NOT NULL,   -- 提前幾天顯示黃燈
    NextDueDate         DATE            NOT NULL,   -- 下次截止日
    CreatedAt           DATETIME2       NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- 8. ReportStatusLogs 報告進度燈號資料表
-- ============================================================
CREATE TABLE ReportStatusLogs (
    Id              INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ScheduleId      INT             NOT NULL,
    LastUpdateDate  DATETIME2       NULL,           -- 最後一次上傳時間
    NextDueDate     DATE            NOT NULL,       -- 本次截止日
    Status          NVARCHAR(10)    NOT NULL CHECK (Status IN (N'Green', N'Yellow', N'Red')),
    CONSTRAINT FK_ReportStatusLogs_Schedules FOREIGN KEY (ScheduleId) REFERENCES ReportSchedules(Id)
);
GO

CREATE INDEX IX_ReportStatusLogs_ScheduleId ON ReportStatusLogs(ScheduleId);
GO

-- ============================================================
-- 初始示範資料
-- ============================================================

-- 預設管理員帳號（密碼：Admin@123，BCrypt 雜湊）
INSERT INTO Users (Username, PasswordHash, Email, RoleId)
VALUES (N'admin', N'$2a$11$rLmeRXe7JUNAzDSuHFiDxuRIUGxSBobMWJGm.jxFKnXgJYD3FkLEq', N'admin@esg.com', 1);

-- 預設一般使用者帳號（密碼：User@123，BCrypt 雜湊）
INSERT INTO Users (Username, PasswordHash, Email, RoleId)
VALUES (N'user01', N'$2a$11$YN3hW0rMV3Tv5fgDkbLnkecRJh5a0D2BqfEqJ7NqTFHM1WQ/mMxLK', N'user01@esg.com', 2);
GO

-- 範疇一 碳排係數示範資料
INSERT INTO EmissionConfigs (Scope, Category, ItemName, Factor, GWP, Unit) VALUES
(1, N'固定燃燒源', N'天然氣',   2.0416,  1.0, N'kg CO2e/m³'),
(1, N'固定燃燒源', N'柴油',     2.6360,  1.0, N'kg CO2e/L'),
(1, N'固定燃燒源', N'液化石油氣', 2.9920, 1.0, N'kg CO2e/L'),
(1, N'移動燃燒源', N'汽油',     2.2637,  1.0, N'kg CO2e/L'),
(1, N'移動燃燒源', N'柴油(車用)', 2.6280, 1.0, N'kg CO2e/L'),
(1, N'逸散排放',   N'冷媒R-22', 1810.0,  1.0, N'kg CO2e/kg');
GO

-- 範疇二 碳排係數示範資料（台灣電力排放係數）
INSERT INTO EmissionConfigs (Scope, Category, ItemName, Factor, GWP, Unit) VALUES
(2, N'外購電力', N'台電電力', 0.4950, 1.0, N'kg CO2e/kWh'),
(2, N'外購蒸汽', N'工業蒸汽', 0.0950, 1.0, N'kg CO2e/MJ');
GO

-- 報告排程示範資料
INSERT INTO ReportSchedules (ReportName, Frequency, ResponsiblePerson, WarningDays, NextDueDate) VALUES
(N'月度碳排放盤查報告', N'Monthly', N'環境管理部-王小明', 7,  DATEADD(MONTH, 1, CAST(GETDATE() AS DATE))),
(N'年度溫室氣體盤查報告', N'Yearly', N'永續發展委員會-李大華', 90, DATEADD(YEAR, 1, CAST(GETDATE() AS DATE))),
(N'供應鏈碳排放月報', N'Monthly', N'採購部-陳小玲', 7, DATEADD(MONTH, 1, CAST(GETDATE() AS DATE)));
GO

PRINT N'ESG 平台資料庫初始化完成！';
PRINT N'預設管理員帳號: admin / Admin@123';
PRINT N'預設一般使用者: user01 / User@123';
GO
