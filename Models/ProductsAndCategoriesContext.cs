using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Models
{
    public class ProductsAndCategoriesContext : DbContext
    {
        public ProductsAndCategoriesContext(DbContextOptions options)
            : base(options) {}

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategoryAssociation> ProductCategoryAssociations { get; set; }
    }
}