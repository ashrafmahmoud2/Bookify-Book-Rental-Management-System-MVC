﻿using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel.Subscriber;
using Bookify.Web.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Humanizer;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Linq.Dynamic.Core;
using static System.Net.Mime.MediaTypeNames;

namespace Bookify.Web.Controllers;

public class BooksController : Controller
{
    //19/7
    //  https://preview.keenthemes.com/metronic8/demo1/pages/user-profile/projects.html



    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly Cloudinary _cloudinary;

    private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
    private int _maxAllowedSize = 2097152;

    public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> cloudinary)
    {
        _context = context;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        Account account = new()
        {
            Cloud = cloudinary.Value.Cloud,
            ApiKey = cloudinary.Value.ApiKey,
            ApiSecret = cloudinary.Value.ApiSecret,
        };

        _cloudinary = new Cloudinary(account);
    }
    public IActionResult Index()
    {
        var books = _context.Books
            .Include(b => b.Author)
            .AsNoTracking().ToList();
        var viewModal = _mapper.Map<IEnumerable<BookViewModel>>(books);
        return View(viewModal);

    }

    [HttpPost,IgnoreAntiforgeryToken]
    public IActionResult GetBooks()
    {
        var skip = int.Parse(Request.Form["start"]);
        var pageSize = int.Parse(Request.Form["length"]);

        var searchValue = Request.Form["search[value]"];

        var sortColumnIndex = Request.Form["order[0][column]"];
        var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
        var sortColumnDirection = Request.Form["order[0][dir]"];

        IQueryable<Book> books = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .ThenInclude(c => c.Category);

        if (!string.IsNullOrEmpty(searchValue))
            books = books.Where(b => b.Title.Contains(searchValue) || b.Author!.Name.Contains(searchValue));

        // Using the System.Linq.Dynamic.Core package allows dynamic ordering by variables. 
        // Without it, you can only use strongly-typed properties, .OrderBy(b => b.PropertyName).

        books = books.OrderBy($"{sortColumn} {sortColumnDirection}");

        var data = books.Skip(skip).Take(pageSize).ToList();

        var mappedData = _mapper.Map<IEnumerable<BookViewModel>>(data);

        var recordsTotal = books.Count();

        var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

        return Ok(jsonData);
    }

    public IActionResult Details(int id)
    {
        var book = _context.Books
         .Include(b => b.Categories)
         .ThenInclude(bc => bc.Category)
         .Include(b => b.Author)
         .Include(b => b.Copies)
         .SingleOrDefault(b => b.Id == id);

        if (book is null)
            return NotFound();

        var bookViewModel = _mapper.Map<BookViewModel>(book);

        return View("Details", bookViewModel);
    }


    public IActionResult Create()
    {
        return View("Form", PopulateViewModel());
    }

    [HttpPost]
   
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel(model));

        var book = _mapper.Map<Book>(model);

        if (model.Image is not null)
        {
            var extension = Path.GetExtension(model.Image.FileName);

            if (!_allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                return View("Form", PopulateViewModel(model));
            }

            if (model.Image.Length > _maxAllowedSize)
            {
                ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                return View("Form", PopulateViewModel(model));
            }

            var imageName = $"{Guid.NewGuid()}{extension}";

            //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

            //using var stream = System.IO.File.Create(path);
            //await model.Image.CopyToAsync(stream);

            //book.ImageUrl = imageName;

            using var stream = model.Image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageName, stream),
                UseFilename = true,
                Folder = "books",
                PublicId = Guid.NewGuid().ToString(),
                Overwrite = false,
            };

            //this will Upload the path of image in cloudinary
            var Result = await _cloudinary.UploadAsync(uploadParams);

            if (Result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(nameof(model.Image), "Image upload failed.");
                return View("Form", PopulateViewModel(model));
            }

            book.ImageUrl = Result.SecureUrl.ToString();
            book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);
            book.ImagePublicId = Result.PublicId;
        }

        book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        foreach (var category in model.SelectedCategories)
            book.Categories.Add(new BookCategory { CategoryId = category });

        _context.Add(book);
        _context.SaveChanges();

        return RedirectToAction(nameof(Details), new { id = book.Id });

    }

    public IActionResult Edit(int id)
    {
        var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);

        if (book is null)
            return NotFound();

        var model = _mapper.Map<BookFormViewModel>(book);
        var viewModel = PopulateViewModel(model);

        viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

        return View("Form", viewModel);
    }

    [HttpPost]
   
    public async Task<IActionResult> Edit(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel(model));

        var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == model.Id);

        if (book is null)
            return NotFound();

        string imagePuplicId = null;
        if (model.Image is not null)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(book.ImageUrl))
            {
                //var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", book.ImageUrl);

                //if (System.IO.File.Exists(oldImagePath))
                //    System.IO.File.Delete(oldImagePath);

                await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
            }

            var extension = Path.GetExtension(model.Image.FileName);

            if (!_allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                return View("Form", PopulateViewModel(model));
            }

            if (model.Image.Length > _maxAllowedSize)
            {
                ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                return View("Form", PopulateViewModel(model));
            }

            var imageName = $"{Guid.NewGuid()}{extension}";

            //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

            //using var stream = System.IO.File.Create(path);
            //await model.Image.CopyToAsync(stream);

            //model.ImageUrl = imageName;

            using var stream = model.Image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageName, stream),
                UseFilename = true,
                Folder = "books",
                PublicId = Guid.NewGuid().ToString(),
                Overwrite = false,
            };

            //this will Upload the path of image in cloudinary
            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(nameof(model.Image), "Image upload failed.");
                return View("Form", PopulateViewModel(model));
            }

            model.ImageUrl = result.SecureUrl.ToString();
            imagePuplicId = result.PublicId;

        }

        else if (!string.IsNullOrEmpty(book.ImageUrl))
        {
            model.ImageUrl = book.ImageUrl;
            model.ImageThumbnailUrl = book.ImageThumbnailUrl;
        }

        book = _mapper.Map(model, book);
        book.LastUpdatedOn = DateTime.Now;
        book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);
        book.ImagePublicId = imagePuplicId;
        book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;




        foreach (var category in model.SelectedCategories)
            book.Categories.Add(new BookCategory { CategoryId = category });


        if (!model.IsAvailableForRental)
        {
            foreach (var copy in book.Copies)
            {
                copy.IsAvailableForRental = false;
            }
        }

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult AllowItem(BookFormViewModel model)
    {
        var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId);
        bool isAllowed = book is null || book.Id.Equals(model.Id);

        return Json(isAllowed);
    }
    private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
    {
        BookFormViewModel viewModel = model is null ? new BookFormViewModel() : model;

        var authors = _context.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
        var categories = _context.Categories.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();

        viewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors);
        viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);

        return viewModel;
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var book = _context.Books.SingleOrDefault(a => a.Id == id);
        if (book is null)
            return NotFound();

        _context.Books.Remove(book);
        _context.SaveChanges();

        return Ok();
    }

    [HttpPut]
    public IActionResult ToggleStatus(int id)
    {
        var book = _context.Books.SingleOrDefault(a => a.Id == id);
        if (book is null)
            return NotFound();

        book.IsDeleted = !book.IsDeleted;
        book.LastUpdatedOn = DateTime.Now;
        book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _context.SaveChanges();
        book.Title = book.Title;
        return NoContent();
    }



    private string GetThumbnailUrl(string url)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("/upload/"))
            return url;

        return url.Replace("/upload/", "/upload/c_thumb,w_200,g_face/");
    }

}


