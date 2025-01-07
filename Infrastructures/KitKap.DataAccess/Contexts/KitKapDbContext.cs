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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
       .Property(b => b.BookPoint)
       .HasPrecision(5, 2); // 18 toplam basamak, 2 ondalıklı basamak

            // PointTransferred için precision ve scale belirleme
            modelBuilder.Entity<Transaction>()
                .Property(t => t.PointTransferred)
                .HasPrecision(8, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
