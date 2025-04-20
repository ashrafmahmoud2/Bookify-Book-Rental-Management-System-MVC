namespace Bookify.Web.Core.ViewModel.BookCopy;

public class BookCopyFormViewModel : BaseModel
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string EditionNumber { get; set; }

    public bool IsAvailableForRental { get; set; }

}
