using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Xml.Serialization;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


       


        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }

        public DbSet<BookCopy> BookCopies { get; set; }

        public DbSet<Category> Categories { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.HasSequence<int>("SerialNumber",schema:"shared")
                .StartsAt(1000);

            builder.Entity<BookCopy>().Property(bc => bc.SerialNumber)
                .HasDefaultValueSql( "NEXT VALUE FOR shared.SerialNumber");

            //composite key 
            builder.Entity<BookCategory>().HasKey(bc => new { bc.BookId, bc.CategoryId });


            base.OnModelCreating(builder);


        }

    }
}