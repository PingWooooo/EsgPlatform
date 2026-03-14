using Dapper;
using EsgPlatform.Models;
using EsgPlatform.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EsgPlatform.Repositories;

/// <summary>報告燈號狀態資料存取實作</summary>
public class ReportStatusLogRepository : IReportStatusLogRepository
{
    private readonly string _connectionString;

    public ReportStatusLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("資料庫連線字串未設定");
    }

    public async Task<IEnumerable<ReportStatusLog>> GetAllWithScheduleInfoAsync()
    {
        const string sql = @"
            SELECT sl.Id, sl.ScheduleId, sl.LastUpdateDate, sl.NextDueDate, sl.Status,
                   rs.ReportName, rs.Frequency, rs.ResponsiblePerson
            FROM ReportStatusLogs sl
            INNER JOIN ReportSchedules rs ON sl.ScheduleId = rs.Id
            ORDER BY sl.NextDueDate";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<ReportStatusLog>(sql);
    }

    public async Task<int> InsertAsync(ReportStatusLog log)
    {
        const string sql = @"
            INSERT INTO ReportStatusLogs (ScheduleId, LastUpdateDate, NextDueDate, Status)
            VALUES (@ScheduleId, @LastUpdateDate, @NextDueDate, @Status);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var conn = new SqlConnection(_connectionString);
        return await conn.ExecuteScalarAsync<int>(sql, log);
    }

    public async Task UpdateStatusAsync(int scheduleId, string status, DateTime? lastUpdateDate)
    {
        const string sql = @"
            UPDATE ReportStatusLogs
            SET Status = @Status, LastUpdateDate = @LastUpdateDate
            WHERE ScheduleId = @ScheduleId";

        using var conn = new SqlConnection(_connectionString);
        await conn.ExecuteAsync(sql, new { ScheduleId = scheduleId, Status = status, LastUpdateDate = lastUpdateDate });
    }

    public async Task<ReportStatusLog?> GetLatestByScheduleIdAsync(int scheduleId)
    {
        const string sql = @"
            SELECT TOP 1 sl.*, rs.ReportName, rs.Frequency, rs.ResponsiblePerson
            FROM ReportStatusLogs sl
            INNER JOIN ReportSchedules rs ON sl.ScheduleId = rs.Id
            WHERE sl.ScheduleId = @ScheduleId
            ORDER BY sl.Id DESC";

        using var conn = new SqlConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<ReportStatusLog>(sql, new { ScheduleId = scheduleId });
    }
}
