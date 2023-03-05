using Microsoft.EntityFrameworkCore;
using StoreWebApi.Models;

public class ApplicationContext : DbContext
{
    private readonly IConfiguration _configuration;
    public ApplicationContext(IConfiguration configuration)
    {
        _configuration= configuration;
        Database.EnsureCreated();
    }
         
    public DbSet<CustomerDb> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<ProductDb> Products { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    }
}