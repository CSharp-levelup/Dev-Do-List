using DevDoListServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Data;

public partial class AppDbContext : DbContext
{
    private readonly string _connectionString;

    public AppDbContext(DbContextOptions<AppDbContext> options, DbConnectionDetails dbConnectionDetails)
        : base(options)
    {
        _connectionString = dbConnectionDetails.ConnectionString;
        Console.WriteLine("\n\n\n\n" + _connectionString);
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<TaskType> TaskTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Comment1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Comment");

            entity.HasOne(d => d.Task).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskComment");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.ToTable("Task");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TaskTypeId).HasColumnName("TaskTypeID");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Status).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusTask");

            entity.HasOne(d => d.TaskType).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TypeTask");

            entity.HasOne(d => d.User).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserTask");
        });

        modelBuilder.Entity<TaskType>(entity =>
        {
            entity.ToTable("TaskType");

            entity.Property(e => e.TaskTypeId).HasColumnName("TaskTypeID");
            entity.Property(e => e.TaskTypeDescription)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User");

            entity.HasIndex(e => e.Username, "UQ_Username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserPicUrl)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
