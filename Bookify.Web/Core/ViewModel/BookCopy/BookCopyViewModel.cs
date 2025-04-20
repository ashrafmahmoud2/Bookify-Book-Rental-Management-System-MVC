namespace Bookify.Web.Core.ViewModel.BookCopy;

public class BookCopyViewModel
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string BookTitle { get; set; } 
    public bool IsAvailableForRental { get; set; }

    public string EditionNumber { get; set; } = string.Empty;

    public int SerialNumber { get; set; }


    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; } 

}
