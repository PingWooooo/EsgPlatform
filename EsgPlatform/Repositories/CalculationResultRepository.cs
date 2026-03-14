using Dapper;
using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EsgPlatform.Repositories;

/// <summary>計算結果資料存取實作</summary>
public class CalculationResultRepository : ICalculationResultRepository
{
    private readonly string _connectionString;

    public CalculationResultRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("資料庫連線字串未設定");
    }

    public async Task<int> InsertAsync(CalculationResult result)
    {
        const string sql = @"
            INSERT INTO CalculationResults (UploadId, TotalCO2e, CalculatedAt)
            VALUES (@UploadId, @TotalCO2e, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, result);
    }

    public async Task<IEnumerable<CalculationResult>> GetAllWithDetailsAsync()
    {
        const string sql = @"
            SELECT cr.Id, cr.UploadId, cr.TotalCO2e, cr.CalculatedAt,
                   r.Scope, r.Category, r.ItemName, r.UploadDate,
                   u.Username
            FROM CalculationResults cr
            INNER JOIN RawDataUploads r ON cr.UploadId = r.Id
            INNER JOIN Users u ON r.UserId = u.Id
            ORDER BY cr.CalculatedAt DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<CalculationResult>(sql);
    }

    public async Task<IEnumerable<CalculationResult>> GetRecentAsync(int count = 20)
    {
        const string sql = @"
            SELECT TOP (@Count) cr.Id, cr.UploadId, cr.TotalCO2e, cr.CalculatedAt,
                   r.Scope, r.Category, r.ItemName, r.UploadDate,
                   u.Username
            FROM CalculationResults cr
            INNER JOIN RawDataUploads r ON cr.UploadId = r.Id
            INNER JOIN Users u ON r.UserId = u.Id
            ORDER BY cr.CalculatedAt DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<CalculationResult>(sql, new { Count = count });
    }
}
