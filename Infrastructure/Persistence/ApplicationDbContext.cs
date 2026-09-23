using Microsoft.EntityFrameworkCore;
using RoleManagementBackend.Domain.Entities;

namespace RoleManagementBackend.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ==========================================
    // DB SETS
    // ==========================================

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Designation> Designations => Set<Designation>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Visitor> Visitors => Set<Visitor>();

    public DbSet<VisitorActivity> VisitorActivities => Set<VisitorActivity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // ==========================================
        // USERS TABLE MAPPING
        // Table: "users"
        // ==========================================

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("Id");

            entity.Property(u => u.Name)
                .HasColumnName("Name");

            entity.Property(u => u.Email)
                .HasColumnName("Email");

            entity.Property(u => u.Password)
                .HasColumnName("Password");

            entity.Property(u => u.Phone)
                .HasColumnName("Phone");

            entity.Property(u => u.Age)
                .HasColumnName("Age");

            entity.Property(u => u.Address)
                .HasColumnName("Address");

            entity.Property(u => u.RoleId)
                .HasColumnName("RoleId");

            entity.Property(u => u.DeletedFlag)
                .HasColumnName("DeletedFlag");

            entity.Property(u => u.DesignationId)
                .HasColumnName("DesignationId");

            entity.Property(u => u.IsFirstLogin)
                .HasColumnName("IsFirstLogin");

            entity.Property(u => u.ProfileImage)
                .HasColumnName("ProfileImage");


            // User -> Role

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);


            // User -> Designation

            entity.HasOne(u => u.Designation)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DesignationId)
                .OnDelete(DeleteBehavior.SetNull);


            // User -> AuditLogs

            entity.HasMany(u => u.AuditLogs)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // ==========================================
        // ROLES TABLE MAPPING
        // Table: "roles"
        // ==========================================

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .HasColumnName("Id");

            entity.Property(r => r.Name)
                .HasColumnName("Name");

            entity.Property(r => r.Description)
                .HasColumnName("Description");
        });


        // ==========================================
        // DESIGNATIONS TABLE MAPPING
        // Table: "designations"
        // ==========================================

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.ToTable("designations");

            entity.HasKey(d => d.Id);

            entity.Property(d => d.Id)
                .HasColumnName("Id");

            entity.Property(d => d.Name)
                .HasColumnName("Name");

            entity.Property(d => d.Description)
                .HasColumnName("Description");

            entity.Property(d => d.DepartmentId)
                .HasColumnName("DepartmentId");


            // Designation -> Department

            entity.HasOne(d => d.Department)
                .WithMany(dep => dep.Designations)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        // ==========================================
        // DEPARTMENTS TABLE MAPPING
        // Table: "departments"
        // ==========================================

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");

            entity.HasKey(dep => dep.Id);

            entity.Property(dep => dep.Id)
                .HasColumnName("Id");

            entity.Property(dep => dep.Name)
                .HasColumnName("Name");
        });


        // ==========================================
        // AUDIT_LOGS TABLE MAPPING
        // Table: "audit_logs"
        // ==========================================

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .HasColumnName("id");

            entity.Property(a => a.UserId)
                .HasColumnName("user_id");

            entity.Property(a => a.Action)
                .HasColumnName("action");

            entity.Property(a => a.EntityType)
                .HasColumnName("entity_type");

            entity.Property(a => a.EntityId)
                .HasColumnName("entity_id");

            entity.Property(a => a.OldValues)
                .HasColumnName("old_values");

            entity.Property(a => a.NewValues)
                .HasColumnName("new_values");

            entity.Property(a => a.Timestamp)
                .HasColumnName("timestamp");

            entity.Property(a => a.IpAddress)
                .HasColumnName("ip_address");

            entity.Property(a => a.UserAgent)
                .HasColumnName("user_agent");
        });


        // ==========================================
        // VISITORS TABLE MAPPING
        // Table: "visitors"
        // ==========================================

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.ToTable("visitors");

            entity.HasKey(v => v.Id);

            entity.Property(v => v.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(v => v.Name)
                .HasColumnName("Name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(v => v.Email)
                .HasColumnName("Email")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(v => v.Phone)
                .HasColumnName("Phone")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(v => v.Company)
                .HasColumnName("Company")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(v => v.Purpose)
                .HasColumnName("Purpose")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(v => v.VisitDate)
                .HasColumnName("VisitDate")
                .HasColumnType("date")
                .IsRequired();

            entity.Property(v => v.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("timestamp without time zone")
                .IsRequired();
        });


        // ==========================================
        // VISITOR ACTIVITIES TABLE MAPPING
        // Table: "visitor_activities"
        // ==========================================

        modelBuilder.Entity<VisitorActivity>(entity =>
        {
            entity.ToTable("visitor_activities");

            entity.HasKey(v => v.Id);

            entity.Property(v => v.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            entity.Property(v => v.VisitorId)
                .HasColumnName("VisitorId")
                .IsRequired();

            entity.Property(v => v.Activity)
                .HasColumnName("Activity")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(v => v.Description)
                .HasColumnName("Description")
                .HasMaxLength(1000);

            entity.Property(v => v.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            entity.HasOne(v => v.Visitor)
                .WithMany(v => v.VisitorActivities)
                .HasForeignKey(v => v.VisitorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}