namespace Bookify.Web.Core.ViewModel.Book;


public class BookViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public string AuthorName { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishingDate { get; set; }
    public string Hall { get; set; }
    public bool IsAvailableForRental { get; set; }
    public string CategoryName { get; set; }
    public bool IsDeleted { get; set; }
}

