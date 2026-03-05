using Microsoft.EntityFrameworkCore;
using WongaLoginService.Models;

namespace WongaLoginService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            // Primary Key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            
            // Indexed columns for fast lookups
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("ix_users_email");
            
            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("ix_users_username");
            
            // Column constraints
            entity.Property(e => e.Username)
                .HasColumnName("username")
                .HasMaxLength(20)
                .IsRequired();
            
            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(60)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
