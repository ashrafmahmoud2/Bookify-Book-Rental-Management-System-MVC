namespace Bookify.Web.Core.ViewModel.Book;

public class BookFormViewModel : BaseModel
{
    public int Id { get; set; }

    [Remote(action: "AllowItem", null, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
    public string Title { get; set; } = null!;


    [Display(Name = "Author")]

    public int AuthorId { get; set; }

    public IEnumerable<SelectListItem>? Authors { get; set; }

    public string Description { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    [Display(Name = "Publishing Date")]
    public DateTime PublishingDate { get; set; }

    public IFormFile? ImageUrl { get; set; } = null!;

    public string Hall { get; set; } = null!;

    [Display(Name = "Is available for rental")]

    public bool IsAvailableForRental { get; set; }


    public IList<int> SelectedCategories { get; set; } = new List<int>(); //we use Ilist not IEnumerable becsue it has index
    public IEnumerable<SelectListItem>? Categories { get; set; }


}







