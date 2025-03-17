namespace Bookify.Web.Core.ViewModel.Book;

public class BookDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string ImageThumbnailUrl { get; set; }
    public string AuthorName { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishingDate { get; set; }
    public string Hall { get; set; }
    public bool IsAvailableForRental { get; set; }
    public List<string> Categories { get; set; } = new();

    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; } 

}

