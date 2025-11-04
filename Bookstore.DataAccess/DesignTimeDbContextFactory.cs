using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BookStore.DataAccess
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            
            var conn = configuration.GetConnectionString("BookstoreConnection")
                       ?? Environment.GetEnvironmentVariable("DESIGNTIME_CONNECTION")
                       ?? "Server=127.0.0.1;Port=3306;Database=bookstore;User=appuser;Password=NewStrongPass!;SslMode=None;";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
            

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}