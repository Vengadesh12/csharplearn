using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RoleManagementBackend.Config;
using System.Text.Json;

namespace RoleManagementBackend.Controllers
{
    [ApiController]
    [Route("api/check")]
    public class CheckController : ControllerBase
    {
        // =========================================================
        // LOGIN API
        // POST: /api/check/login
        // =========================================================
            public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            try
            {
                // =====================================================
                // VALIDATION
                // =====================================================

                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new
                    {
                        success = false,
                        requiresTwoFactor = false,
                        message = "Email and password are required.",
                        data = (object?)null
                    });
                }

                // =====================================================
                // DATABASE CONNECTION
                // =====================================================

                await using var connection =
                    new NpgsqlConnection(DbConfig.Conn);

                await connection.OpenAsync();

                // =====================================================
                // LOGIN QUERY
                // =====================================================

                string loginQuery = @"
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

                // =====================================================
                // EXECUTE LOGIN QUERY
                // =====================================================

                await using var command =
                    new NpgsqlCommand(loginQuery, connection);

                command.Parameters.AddWithValue(
                    "@email",
                    request.Email.Trim()
                );

                await using var reader =
                    await command.ExecuteReaderAsync();

                // =====================================================
                // USER NOT FOUND
                // =====================================================

                if (!await reader.ReadAsync())
                {
                    return Unauthorized(new
                    {
                        success = false,
                        requiresTwoFactor = false,
                        message = "Invalid email or password.",
                        data = (object?)null
                    });
                }

                // =====================================================
                // READ USER DATA
                // =====================================================

                int userId =
                    Convert.ToInt32(reader["user_id"]);

                string name =
                    reader["user_name"]?.ToString() ?? "";

                string email =
                    reader["user_email"]?.ToString() ?? "";

                string passwordHash =
                    reader["user_password"]?.ToString() ?? "";

                int? roleId =
                    reader["user_role_id"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(reader["user_role_id"]);

                int? designationId =
                    reader["user_designation_id"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            reader["user_designation_id"]
                        );

                string roleName =
                    reader["role_name"]?.ToString() ?? "";

                string designationName =
                    reader["designation_name"]?.ToString() ?? "";

                string departmentName =
                    reader["department_name"]?.ToString() ?? "";

                string profileImage =
                    reader["user_profile_image"]?.ToString() ?? "";

                bool isFirstLogin =
                    reader["user_is_first_login"] != DBNull.Value &&
                    Convert.ToBoolean(
                        reader["user_is_first_login"]
                    );

                int deletedFlag =
                    Convert.ToInt32(
                        reader["user_deleted_flag"]
                    );

                // =====================================================
                // USER STATUS CHECK
                // =====================================================

                if (deletedFlag != 1)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        requiresTwoFactor = false,
                        message =
                            "User account is inactive. Please contact administrator.",
                        data = (object?)null
                    });
                }

                // =====================================================
                // PASSWORD VERIFICATION
                // =====================================================

                var passwordHasher =
                    new PasswordHasher<object>();

                var passwordResult =
                    passwordHasher.VerifyHashedPassword(
                        null!,
                        passwordHash,
                        request.Password
                    );

                if (passwordResult ==
                    PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        requiresTwoFactor = false,
                        message = "Invalid email or password.",
                        data = (object?)null
                    });
                }

                // =====================================================
                // READ PERMISSIONS
                // =====================================================

                string permissionsJson =
                    reader["permissions"]?.ToString() ?? "[]";

                var permissions =
                    JsonSerializer.Deserialize<List<string>>(
                        permissionsJson
                    ) ?? new List<string>();

                // =====================================================
                // READ MENUS
                // =====================================================

                string menusJson =
                    reader["menus"]?.ToString() ?? "[]";

                var menus =
                    JsonSerializer.Deserialize<List<object>>(
                        menusJson
                    ) ?? new List<object>();

                // =====================================================
                // JWT TOKEN
                // =====================================================

                string token =
                    GenerateJwtToken(
                        userId,
                        email,
                        roleName,
                        permissions
                    );

                // =====================================================
                // RESPONSE DATA
                // =====================================================

                var data = new
                {
                    id = userId,
                    name = name,
                    email = email,
                    profileImage = profileImage,

                    roleId = roleId,
                    roleName = roleName,

                    departmentName = departmentName,
                    designationName = designationName,

                    permissions = permissions,
                    menus = menus,

                    token = token,

                    isFirstLogin = isFirstLogin
                };

                // =====================================================
                // SUCCESS RESPONSE
                // =====================================================

                return Ok(new
                {
                    success = true,
                    requiresTwoFactor = false,
                    message = "Login successful.",
                    data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    requiresTwoFactor = false,
                    message = "Database connection or query failed.",
                    error = ex.Message,
                    data = (object?)null
                });
            }
        }

        // =========================================================
        // JSON API
        // GET: /api/check/json/{id}
        // =========================================================

        [HttpGet("json/{id}")]
        public async Task<IActionResult> GetUserJson(int id)
        {
            try
            {
                await using var connection =
                    new NpgsqlConnection(DbConfig.Conn);

                await connection.OpenAsync();

                // =====================================================
                // JSON QUERY
                // =====================================================

                string jsonQuery = @"
SELECT json_build_object(
    'id', u.""Id"",
    'name', u.""Name"",
    'email', u.""Email"",
    'phone', u.""Phone"",
    'age', u.""Age"",
    'address', u.""Address"",
    'roleId', u.""RoleId""
)
FROM public.users AS u
WHERE u.""Id"" = @id;
";

                // =====================================================
                // COMMAND
                // =====================================================

                await using var command =
                    new NpgsqlCommand(
                        jsonQuery,
                        connection
                    );

                command.Parameters.AddWithValue(
                    "@id",
                    id
                );

                // =====================================================
                // EXECUTE
                // =====================================================

                var result =
                    await command.ExecuteScalarAsync();

                // =====================================================
                // USER NOT FOUND
                // =====================================================

                if (result == null ||
                    result == DBNull.Value)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "User not found.",
                        data = (object?)null
                    });
                }

                // =====================================================
                // JSON RESULT
                // =====================================================

                string jsonResult =
                    result.ToString() ?? "{}";

                return Content(
                    jsonResult,
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database query failed.",
                    error = ex.Message,
                    data = (object?)null
                });
            }
        }

        // =========================================================
        // GET SINGLE AUDIT RECORD
        // GET: /api/check/audit/{id}
        // =========================================================

        [HttpGet("audit/{id}")]
        public async Task<IActionResult> GetAuditData(int id)
        {
            try
            {
                await using var connection =
                    new NpgsqlConnection(DbConfig.Conn);

                await connection.OpenAsync();

                // =====================================================
                // AUDIT QUERY
                // =====================================================

                string sql = @"
SELECT *
FROM public.audit_logs AS a
WHERE a.id = @id;
";

                // =====================================================
                // COMMAND
                // =====================================================

                await using var command =
                    new NpgsqlCommand(
                        sql,
                        connection
                    );

                command.Parameters.AddWithValue(
                    "@id",
                    id
                );

                // =====================================================
                // EXECUTE QUERY
                // =====================================================

                await using var reader =
                    await command.ExecuteReaderAsync();

                // =====================================================
                // RECORD NOT FOUND
                // =====================================================

                if (!await reader.ReadAsync())
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Audit record not found.",
                        data = (object?)null
                    });
                }

                // =====================================================
                // READ ALL COLUMNS
                // =====================================================

                var auditLog =
                    new Dictionary<string, object?>();

                for (
                    int i = 0;
                    i < reader.FieldCount;
                    i++
                )
                {
                    string columnName =
                        reader.GetName(i);

                    auditLog[columnName] =
                        await reader.IsDBNullAsync(i)
                            ? null
                            : reader.GetValue(i);
                }

                // =====================================================
                // RESPONSE
                // =====================================================

                return Ok(new
                {
                    success = true,
                    message =
                        "Audit record retrieved successfully.",
                    data = auditLog
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database query failed.",
                    error = ex.Message,
                    data = (object?)null
                });
            }
        }

        // =========================================================
        // GET ALL AUDIT RECORDS
        // GET: /api/check/audit/all
        // =========================================================

        [HttpGet("audit/all")]
        public async Task<IActionResult> GetAllAuditData()
        {
            try
            {
                await using var connection =
                    new NpgsqlConnection(DbConfig.Conn);

                await connection.OpenAsync();

                // =====================================================
                // QUERY
                // =====================================================

                string jsonQuery = @"
SELECT COALESCE(
    json_agg(
        json_build_object(
            'id', a.id,
            'userId', a.user_id,
            'action', a.action,
            'entityType', a.entity_type,
            'entityId', a.entity_id,
            'oldValues', a.old_values,
            'newValues', a.new_values,
            'timestamp', a.timestamp,
            'ipAddress', a.ip_address,
            'userAgent', a.user_agent
        )
        ORDER BY a.timestamp DESC
    ),
    '[]'::json
)
FROM public.audit_logs AS a;
";

                // =====================================================
                // COMMAND
                // =====================================================

                await using var command =
                    new NpgsqlCommand(
                        jsonQuery,
                        connection
                    );

                // =====================================================
                // EXECUTE
                // =====================================================

                var result =
                    await command.ExecuteScalarAsync();

                string jsonResult =
                    result?.ToString() ?? "[]";

                // =====================================================
                // RESPONSE
                // =====================================================

                return Content(
                    jsonResult,
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database query failed.",
                    error = ex.Message,
                    data = (object?)null
                });
            }
        }

        // =========================================================
        // JWT TOKEN GENERATION
        // =========================================================

        private string GenerateJwtToken(
            int userId,
            string email,
            string roleName,
            List<string> permissions)
        {
            // Replace this with your actual JWT implementation.
            return "YOUR_JWT_TOKEN_HERE";
        }
    }

    // =============================================================
    // LOGIN REQUEST MODEL
    // =============================================================



    // =============================================================
    // AUDIT LOG MODEL
    // =============================================================

    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public string OldValues { get; set; } = string.Empty;

        public string NewValues { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string IpAddress { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;
    }
}