using Bookify.Web.Core.ViewModel.User;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;

//[Authorize(Roles = AppRoles.Admin)]
public class UsersController : Controller
{

    //22/5

    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);
        return View(viewModel);

         
        
    }

    [HttpGet]
    //[AjaxOnly]
    public async Task<IActionResult> Create()
    {
        var role = await _roleManager.Roles.ToListAsync();

        UserFormViewModel viewModel = new()
        {

            Roles = await _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToListAsync()
        };
        return PartialView("_Form",viewModel);
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Create(AuthorFormViewModel model)
    //{
    //    if (!ModelState.IsValid)
    //        return BadRequest();

    //    var author = _mapper.Map<Author>(model);
    //    _context.Add(author);
    //    _context.SaveChanges();

    //    var viewModel = _mapper.Map<AuthorViewModel>(author);

    //    return PartialView("_AuthorRow", viewModel);
    //}

    //[HttpGet]
    //public IActionResult Edit(int id)
    //{

    //    var author = _context.Authors.FirstOrDefault(c => c.Id == id);
    //    if (author is null)
    //        return NotFound();

    //    var viewModel = _mapper.Map<AuthorFormViewModel>(author);

    //    return PartialView("_Form", viewModel);
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Edit(AuthorFormViewModel model)
    //{

    //    //stop in fix edit
    //    if (!ModelState.IsValid)
    //        return BadRequest();

    //    var author = _context.Authors.FirstOrDefault(a => a.Id == model.Id);
    //    if (author is null)
    //        return NotFound();

    //    author = _mapper.Map(model, author);
    //    author.LastUpdatedOn = DateTime.Now;
    //    _context.SaveChanges();

    //    var viewModel = _mapper.Map<AuthorViewModel>(author);

    //    return PartialView("_AuthorRow", viewModel);
    //}

    //[HttpDelete]
    //public IActionResult Delete(int id)
    //{
    //    var author = _context.Authors.SingleOrDefault(a => a.Id == id);
    //    if (author is null)
    //        return NotFound();

    //    _context.Authors.Remove(author);
    //    _context.SaveChanges();

    //    return NoContent();
    //}

    //[HttpPut]
    //public IActionResult ToggleStatus(int id)
    //{
    //    var author = _context.Authors.SingleOrDefault(a => a.Id == id);
    //    if (author is null)
    //        return NotFound();

    //    author.IsDeleted = !author.IsDeleted;
    //    _context.SaveChanges();

    //    return NoContent();
    //}

    //public IActionResult AllowItem(AuthorFormViewModel model)
    //{
    //    var author = _context.Authors.SingleOrDefault(a => a.Name == model.Name);
    //    bool isAllowed = author is null || author.Id.Equals(model.Id);

    //    return Json(isAllowed);
    //}



}
