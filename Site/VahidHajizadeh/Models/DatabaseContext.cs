using System;
using System.Collections.Generic;
using System.Data.Entity;
namespace Models
{
   public class DatabaseContext:DbContext
    {
        static DatabaseContext()
        {
        System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ActivationCode> ActivationCodes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<ZarinpallAuthority> ZarinpallAuthorities { get; set; }

        public DbSet<Text> Texts { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<OrderDiscount> OrderDiscounts { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<SiteBlogCategory> SiteBlogCategories { get; set; }
        public DbSet<SiteBlog> SiteBlogs { get; set; }
        public DbSet<SiteBlogImage> SiteBlogImages { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Video> Videos { get; set; }
    }
}
