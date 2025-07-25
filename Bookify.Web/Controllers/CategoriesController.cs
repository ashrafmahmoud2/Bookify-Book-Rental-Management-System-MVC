﻿namespace Bookify.Web.Controllers;



[Authorize(Roles = AppRoles.Archive)]

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoriesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var categories = _context.Categories.AsNoTracking().ToList();

        var viewModel = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);

        return View(viewModel);
    }

    [HttpGet]
    [AjaxOnly]
    public IActionResult Create()
    {
        return PartialView("_Form");
    }

    [HttpPost]
   
    public IActionResult Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var category = _mapper.Map<Category>(model);
        category.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _context.Add(category);
        _context.SaveChanges();

        var viewModel = _mapper.Map<CategoryViewModel>(category);

        return PartialView("_CategoryRow", viewModel);
    }

    [HttpGet]
    [AjaxOnly]
    public IActionResult Edit(int id)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
            return NotFound();

        var viewModel = _mapper.Map<CategoryFormViewModel>(category);

        return PartialView("_Form", viewModel);
    }

    [HttpPost]
   
    public IActionResult Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var category = _context.Categories.FirstOrDefault(c => c.Id == model.Id);
        if (category == null)
            return NotFound();

        category = _mapper.Map(model, category);
        category.LastUpdatedOn = DateTime.Now;
        category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        category.LastUpdatedOn = DateTime.Now;

        _context.SaveChanges();

        var viewModel = _mapper.Map<CategoryViewModel>(category);

        return PartialView("_CategoryRow", viewModel);
    }

    [HttpPut]
   
    public IActionResult ToggleStatus(int id)
    {

        //stop in fix toggle and delete then maek book module
        var category = _context.Categories.FirstOrDefault(c => c.Id == id);

        if (category is null)
            return NotFound();

        category.IsDeleted = !category.IsDeleted;
        category.LastUpdatedOn = DateTime.Now;
        category.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        _context.SaveChanges();

        return Ok(category.LastUpdatedOn.ToString());
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var category = _context.Categories.SingleOrDefault(c => c.Id == id);
        if (category == null)
            return NotFound();

        _context.Remove(category);

        _context.SaveChanges();
        return NoContent();

    }

    public IActionResult AllowItem(CategoryFormViewModel model)
    {
        var category = _context.Categories.SingleOrDefault(c => c.Name == model.Name);
        var isAllowed = category is null || category.Id.Equals(model.Id);

        return Json(isAllowed);
    }
}






