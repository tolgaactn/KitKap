using Kitkap.Entity.Entities;
using Kitkap.Entity.Entities.Identity;
using KitKap.DataAccess.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitKap.DataAccess.Contexts
{
    public class KitKapDbContext : IdentityDbContext<AppUser>
    {
        public KitKapDbContext(DbContextOptions<KitKapDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                    .Property(p => p.Price)
                    .HasPrecision(6, 2); // 18 toplam basamak, 2 ondalıklı basamak

            // PointTransferred için precision ve scale belirleme
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Transaction → Order ilişkisi Order tarafında tanımlandı (1-to-1)
                // Burada tekrar yazmaya gerek yok

                // Decimal precision
                entity.Property(t => t.Amount)
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            modelBuilder.Entity<Product>()
                    .HasOne<AppUser>()
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LoginHistory>()
                    .HasOne<AppUser>()
                    .WithMany(u => u.LoginHistories)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(
                
                new Category { Id=1, Name= "Kitap", Description="Kitapların olduğu kategori"},
                new Category { Id=2, Name= "Teknoloji", Description="Teknolojilerin  olduğu kategori"},
                new Category { Id=3, Name= "Roman", Description="Romanların olduğu kategori", ParentCategoryId=1}

                );
            modelBuilder.Entity<About>().HasData(

                new About { AboutId = 1, Description = "as", Address = "dsd", Email = "sdas", Phone = "sdwq" }
                );
            modelBuilder.Entity<Product>()
                    .HasOne<AppUser>()
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne<AppUser>()
                      .WithOne() // her kullanıcının tek sepeti olacak
                      .HasForeignKey<ShoppingCart>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Items)
                      .WithOne(i => i.ShoppingCart)
                      .HasForeignKey(i => i.ShoppingCartId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            // ShoppingCartDetail
            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(i => i.Quantity)
              .IsRequired();

                entity.Property(i => i.UnitPrice)
                       .HasColumnType("decimal(18,2)")
                       .IsRequired();
            });

            // ========================================
            // ORDER İLİŞKİLERİ
            // ========================================

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                // ✅ Order → Buyer (AppUser)
                // Entity'de navigation property YOK ama EF Core ilişkiyi biliyor
                entity.HasOne<AppUser>()  // ← Generic parametre
                    .WithMany()            // User'ın birden fazla siparişi
                    .HasForeignKey(o => o.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ✅ Order → ShippingAddress
                entity.HasOne<Address>()  // ← Generic parametre
                    .WithMany()
                    .HasForeignKey(o => o.ShippingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Order → OrderItems
                entity.HasMany(e => e.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Order → Transaction (1-to-1)
                entity.HasOne(o => o.Transaction)
                    .WithOne(t => t.Order)
                    .HasForeignKey<Transaction>(t => t.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Decimal precision'lar
                entity.Property(o => o.SubTotal).HasPrecision(18, 2);
                entity.Property(o => o.ShippingCost).HasPrecision(18, 2);
                entity.Property(o => o.CommissionAmount).HasPrecision(18, 2);
                entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            });
            // OrderItem
            // ========================================
            // ORDER ITEM İLİŞKİLERİ
            // ========================================

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                // OrderItem → Product
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ✅ OrderItem → Seller (AppUser)
                entity.HasOne<AppUser>()  // ← Generic parametre, navigation property yok
                    .WithMany()
                    .HasForeignKey(e => e.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Validations
                entity.Property(i => i.Quantity).IsRequired();

                // Decimal precision'lar
                entity.Property(i => i.UnitPrice).HasPrecision(18, 2);
                entity.Property(i => i.CommissionRate).HasPrecision(5, 2);
                entity.Property(i => i.CommissionAmount).HasPrecision(18, 2);
                entity.Property(i => i.SellerAmount).HasPrecision(18, 2);
            });


            modelBuilder.Entity<AppUser>()
                      .Property(u => u.Balance)
                      .HasColumnType("decimal(18,2)");
                
            base.OnModelCreating(modelBuilder);
            
            
        }
    }
}
