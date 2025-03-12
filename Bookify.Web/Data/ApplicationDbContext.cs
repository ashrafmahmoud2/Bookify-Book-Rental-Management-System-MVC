using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }

        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            //composite key 
            builder.Entity<BookCategory>()
           .HasKey(bc => new { bc.BookId, bc.CategoryId });


            base.OnModelCreating(builder);


        }

    }
}