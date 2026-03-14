using Dapper;
using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EsgPlatform.Repositories;

/// <summary>法規更新紀錄資料存取實作</summary>
public class RegulationUpdateLogRepository : IRegulationUpdateLogRepository
{
    private readonly string _connectionString;

    public RegulationUpdateLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("資料庫連線字串未設定");
    }

    public async Task<int> InsertAsync(RegulationUpdateLog log)
    {
        const string sql = @"
            INSERT INTO RegulationUpdateLogs (ConfigId, OldValue, NewValue, ChangeReason, UpdateDate)
            VALUES (@ConfigId, @OldValue, @NewValue, @ChangeReason, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, log);
    }

    public async Task<IEnumerable<RegulationUpdateLog>> GetAllWithDetailsAsync()
    {
        const string sql = @"
            SELECT rl.Id, rl.ConfigId, rl.OldValue, rl.NewValue, rl.ChangeReason, rl.UpdateDate,
                   ec.ItemName, ec.Category, ec.Scope
            FROM RegulationUpdateLogs rl
            INNER JOIN EmissionConfigs ec ON rl.ConfigId = ec.Id
            ORDER BY rl.UpdateDate DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<RegulationUpdateLog>(sql);
    }
}
