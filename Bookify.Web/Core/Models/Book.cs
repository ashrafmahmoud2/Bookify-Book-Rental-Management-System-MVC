namespace Bookify.Web.Core.Models;

[Index(nameof(Name), IsUnique = true)]
public class Book : BaseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public string ImageUrl { get; set; } = null!; 

    public DateTime PublishingDate { get; set; } 

    public string Hall { get; set; } = null!;

    public bool IsRental { get; set; }

    public int AuthorId { get; set; }

    public Author Author { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
