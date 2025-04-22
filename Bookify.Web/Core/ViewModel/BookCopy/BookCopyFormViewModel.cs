namespace Bookify.Web.Core.ViewModel.BookCopy;

public class BookCopyFormViewModel : BaseModel
{
    public int Id { get; set; }

    public int BookId { get; set; }


    [Display(Name = "Edition Number") , Range(1,1000)]
    public int EditionNumber { get; set; }

    public bool IsAvailableForRental { get; set; }

    public bool ShowRentalInput { get; set; }

}
