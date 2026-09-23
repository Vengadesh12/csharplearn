using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RoleManagementBackend.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor that ensures EF Core migration history stays synchronized with the actual
/// database tables. If a table (such as 'visitor_activities') has been deleted/dropped from
/// the database directly, the corresponding migration entry in '__EFMigrationsHistory'
/// is cleared so EF Core automatically detects the pending migration and recreates the table.
/// </summary>
public class MigrationHistorySyncInterceptor : DbConnectionInterceptor
{
    private static readonly object _syncLock = new();
    private static DateTime _lastSyncCheck = DateTime.MinValue;
    private static readonly TimeSpan ThrottleInterval = TimeSpan.FromSeconds(3);

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        base.ConnectionOpened(connection, eventData);
        SyncHistoryIfNecessary(connection);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        await SyncHistoryIfNecessaryAsync(connection, cancellationToken);
    }

    private static void SyncHistoryIfNecessary(DbConnection connection)
    {
        lock (_syncLock)
        {
            if (DateTime.UtcNow - _lastSyncCheck < ThrottleInterval)
            {
                return;
            }
            _lastSyncCheck = DateTime.UtcNow;
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = GetSyncSql();
            command.ExecuteNonQuery();
        }
        catch
        {
            // Suppress exceptions if database or history table is not yet initialized
        }
    }

    private static async Task SyncHistoryIfNecessaryAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        lock (_syncLock)
        {
            if (DateTime.UtcNow - _lastSyncCheck < ThrottleInterval)
            {
                return;
            }
            _lastSyncCheck = DateTime.UtcNow;
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = GetSyncSql();
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch
        {
            // Suppress exceptions if database or history table is not yet initialized
        }
    }

    private static string GetSyncSql()
    {
        return @"
DO $$
BEGIN
    -- Check if EF Core migration history table exists
    IF to_regclass('public.""__EFMigrationsHistory""') IS NOT NULL THEN
        -- If visitor_activities table was dropped/deleted, remove its history entry
        -- so EF Core migrations will re-run and recreate the table
        IF to_regclass('public.visitor_activities') IS NULL THEN
            DELETE FROM ""__EFMigrationsHistory"" 
            WHERE ""MigrationId"" LIKE '%CreateVisitorActivitiesTable%';
        END IF;

        -- If samevistorscount table was dropped/deleted, remove its history entry
        -- so EF Core migrations will re-run and recreate the table
        IF to_regclass('public.samevistorscount') IS NULL THEN
            DELETE FROM ""__EFMigrationsHistory"" 
            WHERE ""MigrationId"" LIKE '%samevistorscount%';
        END IF;
    END IF;
END $$;";
    }
}
