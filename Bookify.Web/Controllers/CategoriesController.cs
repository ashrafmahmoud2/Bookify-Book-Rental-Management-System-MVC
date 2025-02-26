using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel;
using Bookify.Web.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.NetworkInformation;

namespace Bookify.Web.Controllers;
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }


    public IActionResult Index()
    {
        return View(_context.Categories.AsNoTracking().ToList());
    }

    [AjaxOnly]
    public IActionResult Create()
    {
        return PartialView("_Form");
        // Returning a partial view to load only the form, without the layout
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();


        if (_context.Categories.Any(c => c.Name == model.Name))
            return Conflict("A category with this name already exists.");


        var category = new Category { Name = model.Name };

        _context.Add(category);
        _context.SaveChanges();

        return PartialView("_CategoryRow",category);
        // Return the new row for  update
    }

    [HttpGet]
    [AjaxOnly]
    public IActionResult Edit(int id)
    {
        var category = _context.Categories.Find(id);

        if (category is null)
            return NotFound();

        var viewModel = new CategoryFormViewModel
        {
            Id = id,
            Name = category.Name
        };

        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var category = _context.Categories.Find(model.Id);

        if (category is null)
            return NotFound();

        category.Name = model.Name;
        category.LastUpdatedOn = DateTime.Now;

        _context.SaveChanges();

        return PartialView("_CategoryRow", category);
    }


    public IActionResult Delete(int id)
    {
        var category = _context.Categories.Find(id);

        if (category is null)
            return NotFound();

        _context.Remove(category);

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }

    [HttpPut]
    public IActionResult ToggleStatus(int id)
    {

        var category = _context.Categories.Find(id);

        if (category is null)
            return NotFound();

        category.LastUpdatedOn = DateTime.Now;
        category.IsDeleted = !category.IsDeleted;

        _context.SaveChanges();
        return Ok(DateTime.Now.ToString("dd/MM/yyyy"));

    }




}

