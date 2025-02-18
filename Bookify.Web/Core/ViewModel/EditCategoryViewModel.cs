namespace Bookify.Web.Core.ViewModel;

public class CategoryFormViewModel
{

    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

}



