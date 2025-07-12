﻿using Bookify.Web.Core.ViewModel.Dashborad;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{

    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DashboardController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public IActionResult Index()
    {
        var numberOfCopies = _context.BookCopies.Count(c => !c.IsDeleted);
        //var numberOfCopies = _context.Books.Count(c => !c.IsDeleted);

        numberOfCopies = numberOfCopies <= 10 ? numberOfCopies : numberOfCopies / 10 * 10;

        var numberOfsubscribers = _context.Subscribers.Count(c => !c.IsDeleted);
        var lastAddedBooks = _context.Books
                            .Include(b => b.Author)
                            .Where(b => !b.IsDeleted)
                            .OrderByDescending(b => b.Id)
                            .Take(8)
                            .ToList();

        var topBooks = _context.RentalCopies
            .Include(c => c.BookCopy)
            .ThenInclude(c => c!.Book)
            .ThenInclude(b => b!.Author)
            .GroupBy(c => new
            {
                c.BookCopy!.BookId,
                c.BookCopy!.Book!.Title,
                c.BookCopy!.Book!.ImageThumbnailUrl,
                AuthorName = c.BookCopy!.Book!.Author!.Name
            })
            .Select(b => new
            {
                b.Key.BookId,
                b.Key.Title,
                b.Key.ImageThumbnailUrl,
                b.Key.AuthorName,
                Count = b.Count()
            })
            .OrderByDescending(b => b.Count)
            .Take(6)
            .Select(b => new BookViewModel
            {
                Id = b.BookId,
                Title = b.Title,
                ImageThumbnailUrl = b.ImageThumbnailUrl,
                Author = b.AuthorName
            })
        .ToList();




        var viewModel = new DashboardViewModel
        {
            NumberOfCopies = numberOfCopies,
            NumberOfSubscribers = numberOfsubscribers,
            LastAddedBooks = _mapper.Map<IEnumerable<BookViewModel>>(lastAddedBooks),
            TopBooks = topBooks
        };

        return View(viewModel);
    }
}
