namespace Bookify.Web.Core.ViewModel.Book;


public class BookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string ImageThumbnailUrl { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public DateTime PublishingDate { get; set; }
    public string Hall { get; set; } = null!;
    public bool IsAvailableForRental { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }

    public IEnumerable<string> Categories { get; set; } = null!;


}

