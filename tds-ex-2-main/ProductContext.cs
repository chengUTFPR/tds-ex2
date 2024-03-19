using Microsoft.EntityFrameworkCore;

namespace tds_ex_2_main;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
}
