using Bookify.Web.Core.ViewModel.BookCopy;

namespace Bookify.Web.Controllers;

public class BookCopiesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookCopiesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        var bookCopies = _context.BookCopies.AsNoTracking().ToList();
        var viewModel = _mapper.Map<IEnumerable<BookCopyViewModel>>(bookCopies);
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return PartialView("_Form");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookCopyFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var bookCopy = _mapper.Map<BookCopy>(model);
        _context.Add(bookCopy);
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookCopyViewModel>(bookCopy);
        return PartialView("_BookCopyRow", viewModel);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var bookCopy = _context.BookCopies.FirstOrDefault(c => c.Id == id);
        if (bookCopy is null)
            return NotFound();

        var viewModel = _mapper.Map<BookCopyFormViewModel>(bookCopy);
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(BookCopyFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var bookCopy = _context.BookCopies.FirstOrDefault(c => c.Id == model.Id);
        if (bookCopy is null)
            return NotFound();

        bookCopy = _mapper.Map(model, bookCopy);
        bookCopy.LastUpdatedOn = DateTime.Now;
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookCopyViewModel>(bookCopy);
        return PartialView("_BookCopyRow", viewModel);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var bookCopy = _context.BookCopies.SingleOrDefault(c => c.Id == id);
        if (bookCopy is null)
            return NotFound();

        _context.BookCopies.Remove(bookCopy);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStatus(int id)
    {
        var bookCopy = _context.BookCopies
            .SingleOrDefault(c => c.Id == id);

        if (bookCopy is null)
            return NotFound();

        bookCopy.IsDeleted = !bookCopy.IsDeleted;
        bookCopy.LastUpdatedOn = DateTime.Now;
        _context.SaveChanges();

        return Ok();
    }

    //public IActionResult AllowItem(BookCopyFormViewModel model)
    //{
    //    var bookCopy = _context.BookCopies.SingleOrDefault(c => c.SerialNumber == model.SerialNumber);
    //    bool isAllowed = bookCopy is null || bookCopy.Id.Equals(model.Id);

    //    return Json(isAllowed);
    //}
}
