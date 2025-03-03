namespace Bookify.Web.Core.ViewModel.Book;

public class BookFormViewModel
{
    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Categroy")]
    [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
    public string Name { get; set; } = null!;


}

