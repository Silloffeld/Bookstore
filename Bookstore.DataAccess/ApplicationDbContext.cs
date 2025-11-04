using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CoverType> CoverTypes => Set<CoverType>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<OrderHeader> OrderHeaders => Set<OrderHeader>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    }

    public static class DbSeeder
    {
        public static async Task Initialize(ApplicationDbContext db, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            if (!await roleManager.RoleExistsAsync(Bookstore.Utility.SD.Role_Admin))
            {
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(Bookstore.Utility.SD.Role_Admin));
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(Bookstore.Utility.SD.Role_Customer));
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(Bookstore.Utility.SD.Role_Company));
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(Bookstore.Utility.SD.Role_Employee));

                // Create admin user
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@bookstore.com",
                    Email = "admin@bookstore.com",
                    Name = "Admin User",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 Admin St",
                    City = "Amsterdam",
                    State = "NH",
                    PostalCode = "1000AA",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, Bookstore.Utility.SD.Role_Admin);
            }

            if (db.Categories.Any()) return;
            
            db.Categories.AddRange(
                new Category { Name = "Programming", DisplayOrder = 2 },
                new Category { Name = "Databases", DisplayOrder = 1 },
                new Category { Name = "Science Fiction", DisplayOrder = 3 }
            );
            
            db.CoverTypes.AddRange(
                new CoverType { Name = "Paperback" },
                new CoverType { Name = "Hardcover" },
                new CoverType { Name = "Softcover" }
            );
            
            db.SaveChanges();
            
            // Add seed products
            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product
                    {
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        Description = "A Handbook of Agile Software Craftsmanship",
                        ISBN = "9780132350884",
                        ListPrice = 45.99,
                        Price = 42.99,
                        Price50 = 40.99,
                        Price100 = 38.99,
                        CategoryId = 1,
                        CoverTypeId = 1,
                        ImageUrl = ""
                    },
                    new Product
                    {
                        Title = "Database Design for Mere Mortals",
                        Author = "Michael J. Hernandez",
                        Description = "A Hands-On Guide to Relational Database Design",
                        ISBN = "9780321884497",
                        ListPrice = 52.99,
                        Price = 49.99,
                        Price50 = 47.99,
                        Price100 = 45.99,
                        CategoryId = 2,
                        CoverTypeId = 2,
                        ImageUrl = ""
                    },
                    new Product
                    {
                        Title = "Dune",
                        Author = "Frank Herbert",
                        Description = "Science fiction masterpiece",
                        ISBN = "9780441172719",
                        ListPrice = 29.99,
                        Price = 27.99,
                        Price50 = 25.99,
                        Price100 = 23.99,
                        CategoryId = 3,
                        CoverTypeId = 3,
                        ImageUrl = ""
                    }
                );
                db.SaveChanges();
            }
        }
    }
}
