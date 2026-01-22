using Microsoft.EntityFrameworkCore;
using RideSharingAPI.Models;

namespace RideSharingAPI.Data;

public class RideSharingContext : DbContext
{
    public RideSharingContext(DbContextOptions<RideSharingContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<Ride> Rides { get; set; }
    public DbSet<DriverLocation> DriverLocations { get; set; }
    public DbSet<Dispute> Disputes { get; set; }
    public DbSet<Banner> Banners { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CurrentContext).HasMaxLength(50);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallets");
            entity.HasKey(e => e.ID);
            entity.HasOne(e => e.User).WithOne(u => u.Wallet).HasForeignKey<Wallet>(e => e.UserID);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.ToTable("WalletTransactions");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Ride>(entity =>
        {
            entity.ToTable("Rides");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Distance).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Passenger).WithMany().HasForeignKey(e => e.PassengerID).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Driver).WithMany().HasForeignKey(e => e.DriverID).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DriverLocation>(entity =>
        {
            entity.ToTable("DriverLocations");
            entity.HasKey(e => e.ID);
        });

        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.ToTable("Disputes");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.ToTable("Banners");
            entity.HasKey(e => e.ID);
        });
    }
}
