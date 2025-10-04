using Ecommerce.Data.Configurations;
using Ecommerce.DataAccess.Configurations;
using Ecommerce.DataAccess.EntitiesConfigurations;
using Ecommerce.Entities.Models;
using Ecommerce.Entities.Models.Auth.Identity;
using Ecommerce.Entities.Models.Auth.Users;
using Ecommerce.Entities.Models.Auth.UserTokens;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.DataAccess.ApplicationContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {
        }

        // Identity-related DbSets
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        // Business entity DbSets
        public DbSet<Client> Clients { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CollisionAlert> CollisionAlerts { get; set; }
        public DbSet<CommissionSetting> CommissionSettings { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<Debris> Debris { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewResponse> ReviewResponses { get; set; }
        public DbSet<Satellite> Satellites { get; set; }
        public DbSet<SatelliteCatalog> SatellitesCatalog { get; set; }

        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Withdrawal> Withdrawals { get; set; }
        public DbSet<ServiceMetricEvent> ServiceMetrics { get; set; }
        public DbSet<Notification>Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceProviderConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new CollisionAlertConfiguration());
            modelBuilder.ApplyConfiguration(new CommissionSettingConfiguration());
            modelBuilder.ApplyConfiguration(new DatasetConfiguration());
            modelBuilder.ApplyConfiguration(new DebrisConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewResponseConfiguration());
            modelBuilder.ApplyConfiguration(new SatelliteConfiguration());
            modelBuilder.ApplyConfiguration(new SatelliteCatalogConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawalConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceMetricEventConfiguration());

            // Additional configurations for Identity if needed
            modelBuilder.Entity<User>()
                .HasOne(u => u.Client)
                .WithOne(c => c.User)
                .HasForeignKey<Client>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.ServiceProvider)
                .WithOne(sp => sp.User)
                .HasForeignKey<ServiceProvider>(sp => sp.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}