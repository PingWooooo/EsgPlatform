using Dapper;
using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EsgPlatform.Repositories;

/// <summary>碳排係數設定存取實作</summary>
public class EmissionConfigRepository : IEmissionConfigRepository
{
    private readonly string _connectionString;

    public EmissionConfigRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("資料庫連線字串未設定");
    }

    public async Task<IEnumerable<EmissionConfig>> GetAllAsync()
    {
        const string sql = "SELECT * FROM EmissionConfigs ORDER BY Scope, Category, ItemName";
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<EmissionConfig>(sql);
    }

    public async Task<EmissionConfig?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM EmissionConfigs WHERE Id = @Id";
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<EmissionConfig>(sql, new { Id = id });
    }

    public async Task<EmissionConfig?> GetByScopeAndItemAsync(int scope, string category, string itemName)
    {
        const string sql = @"
            SELECT * FROM EmissionConfigs
            WHERE Scope = @Scope AND Category = @Category AND ItemName = @ItemName";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<EmissionConfig>(sql,
            new { Scope = scope, Category = category, ItemName = itemName });
    }

    public async Task<int> InsertAsync(EmissionConfig config)
    {
        const string sql = @"
            INSERT INTO EmissionConfigs (Scope, Category, ItemName, Factor, GWP, Unit, UpdatedAt)
            VALUES (@Scope, @Category, @ItemName, @Factor, @GWP, @Unit, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, config);
    }

    public async Task UpdateAsync(EmissionConfig config)
    {
        const string sql = @"
            UPDATE EmissionConfigs
            SET Factor = @Factor, GWP = @GWP, Unit = @Unit, UpdatedAt = GETDATE()
            WHERE Id = @Id";

        using var conn = new SqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, config);
    }

    public async Task DeleteAsync(int id)
    {
        const string sql = "DELETE FROM EmissionConfigs WHERE Id = @Id";
        using var conn = new SqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { Id = id });
    }
}
