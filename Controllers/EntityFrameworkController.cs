using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using RoleManagementBackend.Config;
using RoleManagementBackend.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace RoleManagementBackend.Controllers
{
    [ApiController]
    [Route("api/entityframework")]
    [AllowAnonymous]
    public class EntityFrameworkController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public EntityFrameworkController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // =========================================================
        // 6. GET ALL USERS
        // GET: /api/entityframework/users
        // =========================================================
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    phone = u.Phone,
                    age = u.Age,
                    address = u.Address,
                    roleId = u.RoleId,
                    deletedFlag = u.DeletedFlag,
                    designationId = u.DesignationId,
                    isFirstLogin = u.IsFirstLogin,
                    profileImage = u.ProfileImage
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = users
            });
        }

        // =========================================================
        // 11. SIMPLE SEARCH EXAMPLE
        // GET: /api/entityframework/users/search?name=john
        // Demonstrates: LINQ filtering with Where & PostgreSQL ILIKE (case-insensitive)
        // NOTE: Placed before users/{id} so route "users/search" is matched first
        // =========================================================
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Query parameter 'name' is required."
                });
            }

            var users = await _context.Users
                .AsNoTracking()
                .Where(u => EF.Functions.ILike(u.Name, $"%{name.Trim()}%"))
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    phone = u.Phone,
                    age = u.Age,
                    address = u.Address,
                    roleId = u.RoleId,
                    deletedFlag = u.DeletedFlag,
                    designationId = u.DesignationId,
                    isFirstLogin = u.IsFirstLogin,
                    profileImage = u.ProfileImage
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = users
            });
        }

        // =========================================================
        // 7. GET SINGLE USER
        // GET: /api/entityframework/users/{id}
        // Demonstrates: FirstOrDefaultAsync, 404 handling, projection
        // =========================================================
        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    phone = u.Phone,
                    age = u.Age,
                    address = u.Address,
                    roleId = u.RoleId,
                    deletedFlag = u.DeletedFlag,
                    designationId = u.DesignationId,
                    isFirstLogin = u.IsFirstLogin,
                    profileImage = u.ProfileImage
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"User with ID {id} was not found."
                });
            }

            return Ok(new
            {
                success = true,
                data = user
            });
        }

        // =========================================================
        // 8. GET ALL AUDIT LOGS
        // GET: /api/entityframework/auditlogs
        // Demonstrates: OrderByDescending, ToListAsync
        // =========================================================
        [HttpGet("auditlogs")]
        public async Task<IActionResult> GetAllAuditLogs()
        {
            var logs = await _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(x => x.Timestamp)
                .Select(a => new
                {
                    id = a.Id,
                    userId = a.UserId,
                    action = a.Action,
                    entityType = a.EntityType,
                    entityId = a.EntityId,
                    oldValues = a.OldValues,
                    newValues = a.NewValues,
                    timestamp = a.Timestamp,
                    ipAddress = a.IpAddress,
                    userAgent = a.UserAgent
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = logs
            });
        }

        // =========================================================
        // 9. GET AUDIT LOGS FOR A USER
        // GET: /api/entityframework/users/{id}/auditlogs
        // Demonstrates: EF Core relationship with .Include(x => x.User)
        // =========================================================
        [HttpGet("users/{id:int}/auditlogs")]
        public async Task<IActionResult> GetAuditLogsForUser(int id)
        {
            var logs = await _context.AuditLogs
    .AsNoTracking()
    .Where(x => x.UserId == id)
    .OrderByDescending(x => x.Timestamp)
    .Select(x => new
    {
        id = x.Id,
        userId = x.UserId,
        userName = x.User.Name,
        userEmail = x.User.Email,
        action = x.Action,
        entityType = x.EntityType,
        entityId = x.EntityId,
        oldValues = x.OldValues,
        newValues = x.NewValues,
        timestamp = x.Timestamp,
        ipAddress = x.IpAddress,
        userAgent = x.UserAgent
    })
    .ToListAsync();

            return Ok(new
            {
                success = true,
                data = logs
            });
        }

        // =========================================================
        // 10. GET USER WITH AUDIT LOGS
        // GET: /api/entityframework/users/{id}/with-auditlogs
        // =========================================================
        [HttpGet("users/{id:int}/with-auditlogs")]
        public async Task<IActionResult> GetUserWithAuditLogs(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.AuditLogs)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"User with ID {id} was not found."
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    phone = user.Phone,
                    age = user.Age,
                    address = user.Address,
                    roleId = user.RoleId,
                    deletedFlag = user.DeletedFlag,
                    designationId = user.DesignationId,
                    isFirstLogin = user.IsFirstLogin,
                    profileImage = user.ProfileImage,
                    auditLogs = user.AuditLogs
                        .OrderByDescending(a => a.Timestamp)
                        .Select(a => new
                        {
                            id = a.Id,
                            action = a.Action,
                            entityType = a.EntityType,
                            entityId = a.EntityId,
                            oldValues = a.OldValues,
                            newValues = a.NewValues,
                            timestamp = a.Timestamp,
                            ipAddress = a.IpAddress,
                            userAgent = a.UserAgent
                        })
                        .ToList()
                }
            });
        }

        // =========================================================
        // LOGIN
        // POST: /api/entityframework/login
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
        // JWT TOKEN GENERATION
        // =========================================================
        private string GenerateJwtToken(
            int userId,
            string email,
            string roleName,
            List<string> permissions)
        {
            try
            {
                var key = _configuration["Jwt:Key"];
                if (string.IsNullOrEmpty(key))
                {
                    return "YOUR_JWT_TOKEN_HERE";
                }

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, roleName)
                };

                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        double.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var exp) ? exp : 60
                    ),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch
            {
                return "YOUR_JWT_TOKEN_HERE";
            }
        }
    }
}
