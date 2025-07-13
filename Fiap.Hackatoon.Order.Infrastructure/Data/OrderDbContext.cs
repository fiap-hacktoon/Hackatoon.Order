using Fiap.Hackatoon.Order.Domain.Dtos.Order;
using Fiap.Hackatoon.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Hackatoon.Order.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {                
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql("DefaultConnection",
                                        new MySqlServerVersion(new Version(8, 0, 21)),
                                        mySqlOptions => mySqlOptions.MigrationsAssembly("Fiap.Hackatoon.Order.Infrastructure"));
        }

        public DbSet<OrderEntity> OrderEntity { get; set; }

        public DbSet<OrderProduct> OrderProduct { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.ToTable("Order");

                // Configuração da chave primária
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();

                // Configuração das propriedades
                entity.Property(e => e.ClientId)
                      .IsRequired();

                entity.Property(e => e.OrderStatusId)
                      .IsRequired();

                entity.Property(e => e.EmployeeId)
                      .IsRequired();

                entity.Property(e => e.Accepted);

                entity.Property(e => e.FinalPrice)
                      .IsRequired();
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.ToTable("OrderProduct");

                // Configuração da chave primária
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();

                // Configuração das propriedades
                entity.Property(e => e.OrderId)
                      .IsRequired();

                entity.Property(e => e.ProductId)
                      .IsRequired();

                entity.Property(e => e.Quantity)
                      .IsRequired();

                entity.Property(e => e.OrderPrice)
                      .IsRequired();
            });
        }
    }
}
