using Bookify.Web.Core.ViewModel.Author;

namespace Bookify.Web.Controllers;

public class BooksController : Controller
{
    //https://preview.keenthemes.com/html/metronic/docs/index
    //vidoeo linke: https://www.youtube.com/watch?v=nU5pPH-9icY&list=PL62tSREI9C-e98Y2PGnvJXONlFc56VNtZ&index=8;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BooksController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        var books = _context.Books.
            Include(b => b.Category)
            .Include(b => b.Author)
            .AsNoTracking().ToList();
        var viewModal = _mapper.Map<IEnumerable<BookViewModel>>(books);
        return View(viewModal);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var author = _mapper.Map<Author>(model);
        _context.Add(author);
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookViewModel>(author);

        return PartialView("_BookRow", viewModel);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {

        var author = _context.Books.FirstOrDefault(c => c.Id == id);
        if (author is null)
            return NotFound();

        var viewModel = _mapper.Map<BookFormViewModel>(author);

        return PartialView("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(BookFormViewModel model)
    {

        //stop in fix edit
        if (!ModelState.IsValid)
            return BadRequest();

        var author = _context.Books.FirstOrDefault(a => a.Id == model.Id);
        if (author is null)
            return NotFound();

        author = _mapper.Map(model, author);
        author.LastUpdatedOn = DateTime.Now;
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookViewModel>(author);

        return PartialView("_BookRow", viewModel);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var author = _context.Books.SingleOrDefault(a => a.Id == id);
        if (author is null)
            return NotFound();

        _context.Books.Remove(author);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPut]
    public IActionResult ToggleStatus(int id)
    {
        var author = _context.Books.SingleOrDefault(a => a.Id == id);
        if (author is null)
            return NotFound();

        author.IsDeleted = !author.IsDeleted;
        _context.SaveChanges();

        return NoContent();
    }

    public IActionResult AllowItem(BookFormViewModel model)
    {
        var author = _context.Books.SingleOrDefault(a => a.Name == model.Name);
        bool isAllowed = author is null || author.Id.Equals(model.Id);

        return Json(isAllowed);
    }
}
