using Microsoft.EntityFrameworkCore;
using RoleManagementBackend.Entities;

namespace RoleManagementBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // USERS TABLE MAPPING
            // PostgreSQL table: "users"
            // Columns: PascalCase ("Id", "Name", etc.)
            // ==========================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id).HasColumnName("Id");
                entity.Property(u => u.Name).HasColumnName("Name");
                entity.Property(u => u.Email).HasColumnName("Email");
                entity.Property(u => u.Phone).HasColumnName("Phone");
                entity.Property(u => u.Age).HasColumnName("Age");
                entity.Property(u => u.Address).HasColumnName("Address");
                entity.Property(u => u.RoleId).HasColumnName("RoleId");
                entity.Property(u => u.DeletedFlag).HasColumnName("DeletedFlag");
                entity.Property(u => u.DesignationId).HasColumnName("DesignationId");
                entity.Property(u => u.IsFirstLogin).HasColumnName("IsFirstLogin");
                entity.Property(u => u.ProfileImage).HasColumnName("ProfileImage");

                // Configure relationship: User 1 -> Many AuditLogs
                entity.HasMany(u => u.AuditLogs)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .HasPrincipalKey(u => u.Id);
            });

            // ==========================================
            // AUDIT_LOGS TABLE MAPPING
            // PostgreSQL table: "audit_logs"
            // Columns: snake_case ("id", "user_id", etc.)
            // ==========================================
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("audit_logs");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id).HasColumnName("id");
                entity.Property(a => a.UserId).HasColumnName("user_id");
                entity.Property(a => a.Action).HasColumnName("action");
                entity.Property(a => a.EntityType).HasColumnName("entity_type");
                entity.Property(a => a.EntityId).HasColumnName("entity_id");
                entity.Property(a => a.OldValues).HasColumnName("old_values");
                entity.Property(a => a.NewValues).HasColumnName("new_values");
                entity.Property(a => a.Timestamp).HasColumnName("timestamp");
                entity.Property(a => a.IpAddress).HasColumnName("ip_address");
                entity.Property(a => a.UserAgent).HasColumnName("user_agent");
            });
        }
    }
}
