namespace Bookify.Web.Core.Models;

[Index(nameof(Title), nameof(AuthorId), IsUnique = true)]
public class Book : BaseModel
{

    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime PublishingDate { get; set; }

    public string Hall { get; set; } = null!;

    public bool IsAvailableForRental { get; set; }

    public int AuthorId { get; set; }

    public Author? Author { get; set; }

    //public int CategoryId { get; set; }
    //public Category Category { get; set; } = null!;


    public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
}
