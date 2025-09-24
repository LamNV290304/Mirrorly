using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Mirrorly.Models;

public partial class ProjectExeContext : DbContext
{
    public ProjectExeContext()
    {
    }

    public ProjectExeContext(DbContextOptions<ProjectExeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustomerProfile> CustomerProfiles { get; set; }

    public virtual DbSet<Muaprofile> Muaprofiles { get; set; }

    public virtual DbSet<PortfolioItem> PortfolioItems { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<SlotHaveBook> SlotHaveBooks { get; set; }

    public virtual DbSet<TimeOff> TimeOffs { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkingHour> WorkingHours { get; set; }

    public virtual DbSet<IdentityVerification> IdentityVerifications { get; set; }

    public virtual DbSet<TwoFactorAuth> TwoFactorAuths { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(e => e.AddressLine).HasMaxLength(255);
            entity.Property(e => e.ScheduledStart).HasPrecision(3);

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Customer");

            entity.HasOne(d => d.Mua).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.MuaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_MUA");

            entity.HasOne(d => d.SlotHaveBook).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.SlotHaveBookId)
                .HasConstraintName("FK_Bookings_SlotHaveBook");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.CategoryName, "UQ_Categories_CategoryName").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.Property(e => e.CustomerId).ValueGeneratedNever();
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.DisplayName).HasMaxLength(120);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithOne(p => p.CustomerProfile)
                .HasForeignKey<CustomerProfile>(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerProfiles_User");
        });

        modelBuilder.Entity<Muaprofile>(entity =>
        {
            entity.HasKey(e => e.Muaid);

            entity.ToTable("MUAProfiles");

            entity.Property(e => e.Muaid)
                .ValueGeneratedNever()
                .HasColumnName("MUAId");
            entity.Property(e => e.AddressLine).HasMaxLength(255);
            entity.Property(e => e.DisplayName).HasMaxLength(120);
            entity.Property(e => e.ProfilePublic).HasDefaultValue(true);

            entity.HasOne(d => d.Mua).WithOne(p => p.Muaprofile)
                .HasForeignKey<Muaprofile>(d => d.Muaid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MUAProfiles_User");
        });

        modelBuilder.Entity<PortfolioItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);

            entity.Property(e => e.CreatedAtUtc)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MediaUrl).HasMaxLength(500);
            entity.Property(e => e.Muaid).HasColumnName("MUAId");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Mua).WithMany(p => p.PortfolioItems)
                .HasForeignKey(d => d.Muaid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PortfolioItems_MUA");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "IX_Reviews_CustomerId");

            entity.HasIndex(e => e.MuaId, "IX_Reviews_MuaId");

            entity.HasIndex(e => new { e.BookingId, e.CustomerId }, "UQ_Review_Booking_Customer").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Reviews_Booking");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Customer");

            entity.HasOne(d => d.Mua).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.MuaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_MUA");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("VND")
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(160);

            entity.HasOne(d => d.Category).WithMany(p => p.Services)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Services_Category");

            entity.HasOne(d => d.Mua).WithMany(p => p.Services)
                .HasForeignKey(d => d.MuaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_MUA");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slot__3214EC07A6EBC66C");

            entity.ToTable("Slot");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<SlotHaveBook>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SlotHave__3214EC07F1D9ACB9");

            entity.ToTable("SlotHaveBook");

            entity.HasOne(d => d.Slot).WithMany(p => p.SlotHaveBooks)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK_SlotHaveBook_Slot");
        });

        modelBuilder.Entity<TimeOff>(entity =>
        {
            entity.Property(e => e.EndUtc).HasPrecision(3);
            entity.Property(e => e.Reason).HasMaxLength(200);
            entity.Property(e => e.StartUtc).HasPrecision(3);

            entity.HasOne(d => d.Mua).WithMany(p => p.TimeOffs)
                .HasForeignKey(d => d.MuaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimeOffs_MUA");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Expired).HasColumnType("datetime");
            entity.Property(e => e.Token1)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("Token");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_Users_Username").IsUnique();

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasDefaultValue((byte)1);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<WorkingHour>(entity =>
        {
            entity.HasOne(d => d.Mua).WithMany(p => p.WorkingHours)
                .HasForeignKey(d => d.MuaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkingHours_MUA");
        });

        modelBuilder.Entity<IdentityVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ProcessedByAdmin)
                .WithMany()
                .HasForeignKey(d => d.ProcessedByAdminId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TwoFactorAuth>(entity =>
        {
            entity.HasKey(e => e.TwoFactorId);
            entity.HasIndex(e => e.UserId).IsUnique();

            entity.HasOne(d => d.User)
                .WithOne()
                .HasForeignKey<TwoFactorAuth>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
