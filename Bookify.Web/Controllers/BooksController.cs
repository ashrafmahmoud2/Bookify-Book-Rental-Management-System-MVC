namespace Bookify.Web.Controllers;

public class BooksController : Controller
{


    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BooksController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        var books = _context.Books
            //.Include(b => b.Category)
            .Include(b => b.Author)
            .AsNoTracking().ToList();
        var viewModal = _mapper.Map<IEnumerable<BookViewModel>>(books);
        return View(viewModal);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var categories = _context.Categories.Where(c => !c.IsDeleted).OrderBy(a => a.Name).AsNoTracking().ToList();
        var authors = _context.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).AsNoTracking().ToList();

        var viewModel = new BookFormViewModel
        {
            //we do a mapping in MappingProfile for SelectListItem and categories and author to make key = id , value = name
            Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
            Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
        };

        //return View("Form", viewModel);

        return Ok();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookFormViewModel model)
    {


        if (!ModelState.IsValid)
            return BadRequest();

        var book = _mapper.Map<Book>(model);
        book.ImageUrl = "https://example.com/pride_and_prejudice_cover.jpg";
        _context.Add(book);
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookViewModel>(book);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {

        var book = _context.Books.Include(b => b.Author)
            //.Include(b => b.Category)
            .FirstOrDefault(c => c.Id == id);
        if (book is null)
            return NotFound();

        var viewModel = _mapper.Map<BookFormViewModel>(book);

        //viewModel.Authors = _context.Authors.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();
        //viewModel.Categories = _context.Categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();


        return View("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var book = _context.Books.Include(b => b.Author)
            //.Include(b => b.Category)
            .FirstOrDefault(c => c.Id == model.Id);
        if (book is null)
            return NotFound();

        book = _mapper.Map(model, book);
        book.LastUpdatedOn = DateTime.Now;
        _context.SaveChanges();

        var viewModel = _mapper.Map<BookViewModel>(book);

        return RedirectToAction(nameof(Index));
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
        var book = _context.Books.SingleOrDefault(a => a.Title == model.Title);
        bool isAllowed = book is null || book.Id.Equals(model.Id);

        return Json(isAllowed);
    }
}


//model = new BookFormViewModel
//        {
//            Name = model.Name,
//            Description = model.Description,
//            Publisher = model.Publisher,
//            //ImageUrl = "https://example.com/pride_and_prejudice_cover.jpg", // Replace with a real URL
//            PublishingDate = model.PublishingDate,
//            Hall = model.Hall,
//            IsAvailableForRental = model.IsAvailableForRental,
//            AuthorId = 3, // Assuming Jane Austen's ID is 1
//            CategoryId = 104, // Assuming Romance category's ID is 2
//            Authors = new List<SelectListItem>
//            {
//                new SelectListItem { Value = "1", Text = "Jane Austen" },
//                // Add other potential authors if needed
//            },
//            Categories = new List<SelectListItem>
//            {
//                new SelectListItem { Value = "2", Text = "Romance" },
//                new SelectListItem { Value = "3", Text = "Classic Literature" }
//                // Add other potential categories if needed
//            }
//        };