using System.Data;
using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RoleManagementBackend.Application.DTOs;
using RoleManagementBackend.Application.Interfaces;
using RoleManagementBackend.Domain.Entities;
using RoleManagementBackend.Infrastructure.Persistence;

namespace RoleManagementBackend.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Designation)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Designation)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Designation)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> SearchByNameAsync(string name)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Designation)
            .Where(u => EF.Functions.ILike(u.Name, $"%{name.Trim()}%"))
            .ToListAsync();
    }

    public async Task<User?> GetUserWithAuditLogsAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Include(u => u.Designation)
            .Include(u => u.AuditLogs)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserAuthDetails?> GetAuthDetailsByEmailAsync(string email)
    {
        Console.WriteLine("\n................................................................................");
        Console.WriteLine(">>> [STEP 3: REPOSITORY - FETCHING DATA FROM DATABASE]");
        Console.WriteLine("    [FILE]     : Infrastructure/Persistence/Repositories/UserRepository.cs");
        Console.WriteLine("    [METHOD]   : GetAuthDetailsByEmailAsync(string email)");
        Console.WriteLine($"   [QUERY FOR]: Email = '{email}'");
        Console.WriteLine("    [ACTION]   : Preparing raw SQL query with User, Role, Designation, Dept, Permissions, Menus...");

        const string loginQuery = @"
SELECT
    u.""Id"" AS user_id,
    u.""Name"" AS user_name,
    u.""Email"" AS user_email,
    u.""Password"" AS user_password,
    u.""Phone"" AS user_phone,
    u.""Age"" AS user_age,
    u.""Address"" AS user_address,
    u.""RoleId"" AS user_role_id,
    u.""DeletedFlag"" AS user_deleted_flag,
    u.""DesignationId"" AS user_designation_id,
    u.""IsFirstLogin"" AS user_is_first_login,
    u.""ProfileImage"" AS user_profile_image,

    r.""Id"" AS role_id,
    r.""Name"" AS role_name,
    r.""Description"" AS role_description,

    d.""Id"" AS designation_id,
    d.""Name"" AS designation_name,
    d.""Description"" AS designation_description,
    d.""DepartmentId"" AS designation_department_id,

    dep.""Id"" AS department_id,
    dep.""Name"" AS department_name,

    COALESCE(
        (
            SELECT json_agg(
                DISTINCT p.""PermissionKey""
                ORDER BY p.""PermissionKey""
            )
            FROM public.rolepermissions AS rp
            INNER JOIN public.permissions AS p
                ON rp.""PermissionId"" = p.""Id""
            WHERE rp.""RoleId"" = u.""RoleId""
              AND p.""DeletedFlag"" = 1
        ),
        '[]'::json
    ) AS permissions,

    COALESCE(
        (
            SELECT json_agg(
                json_build_object(
                    'id', m.id,
                    'menukey', m.menukey,
                    'label', m.label,
                    'icon', m.icon,
                    'route', m.route,
                    'groupname', m.groupname,
                    'description', m.description,
                    'orderindex', m.orderindex,
                    'permissionkey', m.permissionkey,
                    'deletedflag', m.deletedflag
                )
                ORDER BY m.orderindex, m.id
            )
            FROM public.menus AS m
            WHERE m.deletedflag = 1
              AND
              (
                    LOWER(TRIM(COALESCE(r.""Name"", ''))) = 'super admin'
                    OR m.permissionkey IS NULL
                    OR TRIM(m.permissionkey) = ''
                    OR EXISTS
                    (
                        SELECT 1
                        FROM public.rolepermissions AS rp2
                        INNER JOIN public.permissions AS p2
                            ON rp2.""PermissionId"" = p2.""Id""
                        WHERE rp2.""RoleId"" = u.""RoleId""
                          AND p2.""PermissionKey"" = m.permissionkey
                          AND p2.""DeletedFlag"" = 1
                    )
              )
        ),
        '[]'::json
    ) AS menus
FROM public.users AS u
LEFT JOIN public.roles AS r
    ON u.""RoleId"" = r.""Id""
LEFT JOIN public.designations AS d
    ON u.""DesignationId"" = d.""Id""
LEFT JOIN public.departments AS dep
    ON d.""DepartmentId"" = dep.""Id""
WHERE u.""Email"" = @email;
";

        var connection = _context.Database.GetDbConnection();
        bool wasClosed = connection.State != ConnectionState.Open;

        if (wasClosed)
        {
            Console.WriteLine("    [DB STATUS]: Opening PostgreSQL connection...");
            await connection.OpenAsync();
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = loginQuery;

            var param = command.CreateParameter();
            param.ParameterName = "@email";
            param.Value = email;
            command.Parameters.Add(param);

            Console.WriteLine("    [DB ACTION]: Executing SQL query against PostgreSQL...");
            await using var reader = await command.ExecuteReaderAsync();

            bool rowFound = await reader.ReadAsync();
            Console.WriteLine($"    [DB RESULT]: Record found in database? {rowFound}");

            if (!rowFound)
            {
                Console.WriteLine("    [RESULT]   : No record found -> Returning null to AuthService.cs");
                Console.WriteLine("................................................................................");
                return null;
            }

            Console.WriteLine("    [ACTION]   : Mapping SQL result columns to UserAuthDetails object...");
            var user = new User
            {
                Id = Convert.ToInt32(reader["user_id"]),
                Name = reader["user_name"]?.ToString() ?? string.Empty,
                Email = reader["user_email"]?.ToString() ?? string.Empty,
                Password = reader["user_password"]?.ToString() ?? string.Empty,
                Phone = reader["user_phone"] == DBNull.Value ? null : reader["user_phone"]?.ToString(),
                Age = reader["user_age"] == DBNull.Value ? null : Convert.ToInt32(reader["user_age"]),
                Address = reader["user_address"] == DBNull.Value ? null : reader["user_address"]?.ToString(),
                RoleId = reader["user_role_id"] == DBNull.Value ? null : Convert.ToInt32(reader["user_role_id"]),
                DeletedFlag = Convert.ToInt32(reader["user_deleted_flag"]),
                DesignationId = reader["user_designation_id"] == DBNull.Value ? null : Convert.ToInt32(reader["user_designation_id"]),
                IsFirstLogin = reader["user_is_first_login"] != DBNull.Value && Convert.ToBoolean(reader["user_is_first_login"]),
                ProfileImage = reader["user_profile_image"] == DBNull.Value ? null : reader["user_profile_image"]?.ToString()
            };

            string? roleName = reader["role_name"] == DBNull.Value ? null : reader["role_name"]?.ToString();
            string? designationName = reader["designation_name"] == DBNull.Value ? null : reader["designation_name"]?.ToString();
            string? departmentName = reader["department_name"] == DBNull.Value ? null : reader["department_name"]?.ToString();

            string permissionsJson = reader["permissions"]?.ToString() ?? "[]";
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson) ?? new List<string>();

            string menusJson = reader["menus"]?.ToString() ?? "[]";
            var menus = JsonSerializer.Deserialize<List<object>>(menusJson) ?? new List<object>();

            Console.WriteLine($"    [MAPPED]   : User='{user.Name}' (Id={user.Id}), Role='{roleName}', Permissions={permissions.Count}, Menus={menus.Count}");
            Console.WriteLine("    [RETURN]   : Returning UserAuthDetails to AuthService.cs");
            Console.WriteLine("................................................................................");

            return new UserAuthDetails
            {
                User = user,
                RoleName = roleName,
                DesignationName = designationName,
                DepartmentName = departmentName,
                Permissions = permissions,
                Menus = menus
            };
        }
        finally
        {
            if (wasClosed)
            {
                Console.WriteLine("    [DB STATUS]: Closing PostgreSQL connection.");
                await connection.CloseAsync();
            }
        }
    }
}
