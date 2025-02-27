using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModel;

public class CategoryFormViewModel
{

    public int Id { get; set; }

    [MaxLength(100)]
    [Remote("AllowItem", null,AdditionalFields ="Id", ErrorMessage ="Category with same name is oredy exsits!")]
    public string Name { get; set; } = null!;

}



