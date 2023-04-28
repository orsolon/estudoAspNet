using Microsoft.EntityFrameworkCore;
//fazer com que o Product seja salvo no BD
//precisamos fazer a classe db context



public class ApplicationDBContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories {get; set;}

   public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) {}
   
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
            builder.Entity<Product>()
            .Property(p => p.Name).HasMaxLength(120).IsRequired();
            builder.Entity<Product>()
            .Property(p => p.Code).HasMaxLength(20).IsRequired();
            builder.Entity<Category>()
             .ToTable("Categories");
    }

    //esse metodo sai daqui e vai para o construtor

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // => optionsBuilder.UseSqlServer("Server=localhost;Database=Products;User Id=sa;Password=<Letscode@82>;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");
}