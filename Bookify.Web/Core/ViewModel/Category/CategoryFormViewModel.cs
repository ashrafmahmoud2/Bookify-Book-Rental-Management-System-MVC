using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModel.Category;

public class CategoryFormViewModel
{

    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Categroy")]
    [Remote("AllowItem", null, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
    public string Name { get; set; } = null!;

}




