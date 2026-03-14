using Dapper;
using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EsgPlatform.Repositories;

/// <summary>報告排程資料存取實作</summary>
public class ReportScheduleRepository : IReportScheduleRepository
{
    private readonly string _connectionString;

    public ReportScheduleRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("資料庫連線字串未設定");
    }

    public async Task<IEnumerable<ReportSchedule>> GetAllAsync()
    {
        const string sql = "SELECT * FROM ReportSchedules ORDER BY NextDueDate";
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<ReportSchedule>(sql);
    }

    public async Task<ReportSchedule?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM ReportSchedules WHERE Id = @Id";
        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<ReportSchedule>(sql, new { Id = id });
    }

    public async Task<int> InsertAsync(ReportSchedule schedule)
    {
        const string sql = @"
            INSERT INTO ReportSchedules (ReportName, Frequency, ResponsiblePerson, WarningDays, NextDueDate, CreatedAt)
            VALUES (@ReportName, @Frequency, @ResponsiblePerson, @WarningDays, @NextDueDate, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, schedule);
    }

    public async Task UpdateAsync(ReportSchedule schedule)
    {
        const string sql = @"
            UPDATE ReportSchedules
            SET ReportName = @ReportName,
                Frequency = @Frequency,
                ResponsiblePerson = @ResponsiblePerson,
                WarningDays = @WarningDays,
                NextDueDate = @NextDueDate
            WHERE Id = @Id";

        using var conn = new SqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, schedule);
    }

    public async Task DeleteAsync(int id)
    {
        // 先刪除關聯的燈號紀錄
        const string deleteLogs = "DELETE FROM ReportStatusLogs WHERE ScheduleId = @Id";
        const string deleteSchedule = "DELETE FROM ReportSchedules WHERE Id = @Id";

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();
        try
        {
            await conn.ExecuteAsync(deleteLogs, new { Id = id }, tx);
            await conn.ExecuteAsync(deleteSchedule, new { Id = id }, tx);
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
