using Bookify.Web.Core.ViewModel.Author;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;


public class AuthorsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthorsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        var authors = _context.Authors.AsNoTracking().ToList();
        var viewModal = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
        return View(viewModal);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_Form");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AuthorFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var author = _mapper.Map<Author>(model);
        _context.Add(author);
        _context.SaveChanges();

        var viewModel = _mapper.Map<AuthorViewModel>(author);

        return PartialView("_AuthorRow", viewModel);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {

        var author = _context.Authors.FirstOrDefault(c => c.Id == id);
        if (author is null)
            return NotFound();

        var viewModel = _mapper.Map<AuthorFormViewModel>(author);

        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(AuthorFormViewModel model)
    {

        //stop in fix edit
        if (!ModelState.IsValid)
            return BadRequest();

        var author = _context.Authors.FirstOrDefault(a => a.Id == model.Id);
        if (author is null)
            return NotFound();

        author = _mapper.Map(model, author);
        author.LastUpdatedOn = DateTime.Now;
        _context.SaveChanges();

        var viewModel = _mapper.Map<AuthorViewModel>(author);

        return PartialView("_AuthorRow", viewModel);
    }
  
    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var author = _context.Authors.SingleOrDefault(a => a.Id == id);
        if (author is null)
            return NotFound(); 

        _context.Authors.Remove(author);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPut]
    public IActionResult ToggleStatus(int id)
    {
        var author = _context.Authors.SingleOrDefault(a => a.Id == id);
        if (author is null)
            return NotFound();

        author.IsDeleted = !author.IsDeleted;
        _context.SaveChanges();

        return NoContent();
    }

    public IActionResult AllowItem(AuthorFormViewModel model)
    {
        var author = _context.Authors.SingleOrDefault(a => a.Name == model.Name);
        bool isAllowed = author is null  || author.Id.Equals(model.Id);

        return Json(isAllowed);
    }
}
