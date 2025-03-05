using Bookify.Web.Core.ViewModel.Author;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        var categories = _context.Categories.AsNoTracking().ToList();
        var authors = _context.Authors.AsNoTracking().ToList();

        var viewModel = new BookFormViewModel
        {
            Authors = authors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList(),
            Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
        };

        return View("Form", viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookFormViewModel model)
    {
        //stop in get whey he give me BadRequest whene create a book



        if (!ModelState.IsValid)
        return BadRequest();

        var book = _mapper.Map<Book>(model);
        _context.Add(book);
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookViewModel>(book);

        return View("Form");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {

         var book = _context.Books.FirstOrDefault(c => c.Id == id);
        if (book is null)
            return NotFound();

        var viewModel = _mapper.Map<BookFormViewModel>(book);

        return PartialView("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(BookFormViewModel model)
    {

        //stop in fix edit
        if (!ModelState.IsValid)
            return BadRequest();

         var book = _context.Books.FirstOrDefault(a => a.Id == model.Id);
        if (book is null)
            return NotFound();

        book = _mapper.Map(model, book );
        book.LastUpdatedOn = DateTime.Now;
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookViewModel>(book);

        return PartialView("_BookRow", viewModel);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
         var book = _context.Books.SingleOrDefault(a => a.Id == id);
        if (book is null)
            return NotFound();

        _context.Books.Remove(book);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPut]
    public IActionResult ToggleStatus(int id)
    {
         var book = _context.Books.SingleOrDefault(a => a.Id == id);
        if (book is null)
            return NotFound();

        book.IsDeleted = !book.IsDeleted;
        _context.SaveChanges();

        return NoContent();
    }

    public IActionResult AllowItem(BookFormViewModel model)
    {
         var book = _context.Books.SingleOrDefault(a => a.Name == model.Name);
        bool isAllowed = book is null || book.Id.Equals(model.Id);

        return Json(isAllowed);
    }
}
