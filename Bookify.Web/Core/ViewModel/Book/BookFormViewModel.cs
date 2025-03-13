using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel.Book;

public class BookFormViewModel : BaseModel
{
    public int Id { get; set; }

    [Remote(action: "AllowItem", null, AdditionalFields = "Id,AuthorId", ErrorMessage = Errors.Duplicated)]
    public string Title { get; set; } = null!;


    [Display(Name = "Author")]
    [Remote(action: "AllowItem", null, AdditionalFields = "Id,Title", ErrorMessage = Errors.Duplicated)]
    public int AuthorId { get; set; }

    public IEnumerable<SelectListItem>? Authors { get; set; }

    public string Description { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    [Display(Name = "Publishing Date")]
    [AssertThat("PublishingDate <= Now()",ErrorMessage =Errors.FutureDateNotAllowed)]
    public DateTime PublishingDate { get; set; } =DateTime.Now; //to be a default value in the view;

    public IFormFile? Image  { get; set; } = null!;
    public string? ImageUrl { get; set; } = null!;

    public string Hall { get; set; } = null!;

    [Display(Name = "Is available for rental")]

    public bool IsAvailableForRental { get; set; }

    [Display(Name = "Categories")]
    public IList<int> SelectedCategories { get; set; } = new List<int>(); //we use Ilist not IEnumerable becsue it has index
    public IEnumerable<SelectListItem>? Categories { get; set; }


}







