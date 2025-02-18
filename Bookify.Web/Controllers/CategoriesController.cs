using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel;
using Microsoft.EntityFrameworkCore;

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

    public IActionResult Create()
    {
        return View("Form");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form",model);


        var category = new Category { Name = model.Name };

        _context.Add(category);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
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

        return View("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", model);

        var category = _context.Categories.Find(model.Id);

        if (category is null)
            return NotFound();

        category.Name = model.Name;
        category.LastUpdatedOn = DateTime.Now;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
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
        0
    }


    //stop in section 8
    // in see this vido https://www.youtube.com/watch?v=Vb4md5w6JJ8



}

//the theme link https://preview.keenthemes.com/html/metronic/docs/icons/keenicons