using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.ViewModel.Book;

//public class BookFormViewModel
//{
//    public int Id { get; set; }

//    [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Categroy")]
//    [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
//    public string Name { get; set; } = null!;


//}


[Index(nameof(Name), IsUnique = true)]
public class BookFormViewModel : BaseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public DateTime PublishingDate { get; set; }

    public string Hall { get; set; } = null!;

    public bool IsRental { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public List<SelectListItem> Authors { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();
}







