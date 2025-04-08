using Kitkap.Entity.Entities;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
       .Property(p => p.Price)
       .HasPrecision(6, 2); // 18 toplam basamak, 2 ondalıklı basamak

            // PointTransferred için precision ve scale belirleme
            modelBuilder.Entity<Transaction>()
                    .Property(t => t.PointTransferred)
                    .HasPrecision(8, 2);

            modelBuilder.Entity<Product>()
                    .HasOne<AppUser>()
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(
                
                new Category { Id=1, Name= "Kitap", Description="Kitapların olduğu kategori"},
                new Category { Id=2, Name= "Teknoloji", Description="Teknolojilerin  olduğu kategori"},
                new Category { Id=3, Name= "Roman", Description="Romanların olduğu kategori", ParentCategoryId=1}

                );
            modelBuilder.Entity<About>().HasData(

                new About { AboutId = 1, Description = "as", Address = "dsd", Email = "sdas", Phone = "sdwq" }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
