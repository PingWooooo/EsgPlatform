using Dapper;
using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EsgPlatform.Repositories;

/// <summary>原始排放數據存取實作</summary>
public class RawDataRepository : IRawDataRepository
{
    private readonly string _connectionString;

    public RawDataRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("資料庫連線字串未設定");
    }

    public async Task<int> InsertAsync(RawDataUpload data)
    {
        const string sql = @"
            INSERT INTO RawDataUploads (UserId, Scope, Category, ItemName, Value, Unit, UploadDate)
            VALUES (@UserId, @Scope, @Category, @ItemName, @Value, @Unit, @UploadDate);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, data);
    }

    public async Task<IEnumerable<RawDataUpload>> GetRecentAsync(int count = 50)
    {
        const string sql = @"
            SELECT TOP (@Count) r.*, u.Username
            FROM RawDataUploads r
            INNER JOIN Users u ON r.UserId = u.Id
            ORDER BY r.UploadDate DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<RawDataUpload>(sql, new { Count = count });
    }

    public async Task<IEnumerable<RawDataUpload>> GetByScopeAsync(int scope)
    {
        const string sql = @"
            SELECT r.*, u.Username
            FROM RawDataUploads r
            INNER JOIN Users u ON r.UserId = u.Id
            WHERE r.Scope = @Scope
            ORDER BY r.UploadDate DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<RawDataUpload>(sql, new { Scope = scope });
    }

    public async Task<IEnumerable<RawDataUpload>> GetAllAsync()
    {
        const string sql = @"
            SELECT r.*, u.Username
            FROM RawDataUploads r
            INNER JOIN Users u ON r.UserId = u.Id
            ORDER BY r.UploadDate DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<RawDataUpload>(sql);
    }

    public async Task<Dictionary<int, decimal>> GetScopeCO2eSummaryAsync()
    {
        // 將各範疇的 CO2e 加總（JOIN CalculationResults）
        const string sql = @"
            SELECT r.Scope, SUM(c.TotalCO2e) AS Total
            FROM CalculationResults c
            INNER JOIN RawDataUploads r ON c.UploadId = r.Id
            GROUP BY r.Scope";

        using var conn = new SqlConnection(_connectionString);
        var rows = await conn.QueryAsync<(int Scope, decimal Total)>(sql);

        return rows.ToDictionary(x => x.Scope, x => x.Total);
    }
}
