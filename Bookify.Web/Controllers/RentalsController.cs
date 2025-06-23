using Bookify.Web.Core.Enums;
using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel.Rental;
using Bookify.Web.Core.ViewModel.Subscriber;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bookify.Web.Controllers;

[Authorize(Roles = AppRoles.Reception)]
public class RentalsController : Controller
{
    
    //32 / 6

    //in MarkAsDeleted 
    //if we delete rentel , but RentalCopies don't save tha we don't delete 
    //in this case
    //var copyIsInRental = _context.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);
    //so if we dalat we shoud give ReturnDate value;


    private readonly IDataProtector _dataProtector;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RentalsController(IDataProtectionProvider dataProtector, ApplicationDbContext context, IMapper mapper)
    {
        _dataProtector = dataProtector.CreateProtector("MySecuerKey");
        _context = context;
        _mapper = mapper;
    }

    public IActionResult Create(string sKey)
    {
        var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));

        var subscriber = _context.Subscribers
            .Include(s => s.Subscriptions)
            .Include(s => s.Rentals)
            .ThenInclude(r => r.RentalCopies)
            .SingleOrDefault(s => s.Id == subscriberId);

        if (subscriber is null)
            return NotFound();

        var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber);

        if (!string.IsNullOrEmpty(errorMessage))
            return View("NotAllowedRental", errorMessage);

        var viewModel = new RentalFormViewModel
        {
            SubscriberKey = sKey,
            MaxAllowedCopies = maxAllowedCopies
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RentalFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));

        var subscriber = _context.Subscribers
            .Include(s => s.Subscriptions)
            .Include(s => s.Rentals)
            .ThenInclude(r => r.RentalCopies)
            .SingleOrDefault(s => s.Id == subscriberId);

        if (subscriber is null)
            return NotFound();

        var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber);

        if (!string.IsNullOrEmpty(errorMessage))
            return View("NotAllowedRental", errorMessage);

        var selectedCopies = _context.BookCopies
            .Include(c => c.Book)
            .Include(c => c.Rentals)
            .Where(c => model.SelectedCopies.Contains(c.SerialNumber))
            .ToList();

        var currentSubscriberRentals = _context.Rentals.Where(r => !r.IsDeleted)
            .Include(r => r.RentalCopies)
            .ThenInclude(c => c.BookCopy)
            .Where(r => r.SubscriberId == subscriberId)
            .SelectMany(r => r.RentalCopies)
            .Where(c => !c.ReturnDate.HasValue)
            .Select(c => c.BookCopy!.BookId)
            .ToList();




        List<RentalCopy> copies = new();

        foreach (var copy in selectedCopies)
        {
            if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                return View("NotAllowedRental", Errors.NotAvilableRental);

            if (copy.Rentals.Any(c => !c.ReturnDate.HasValue))
                return View("NotAllowedRental", Errors.CopyIsInRental);

            if (currentSubscriberRentals.Any(bookId => bookId == copy.BookId))
                return View("NotAllowedRental", $"This subscriber already has a copy for '{copy.Book.Title}' Book");

            copies.Add(new RentalCopy { BookCopyId = copy.Id });
        }

        Rental rental = new()
        {
            RentalCopies = copies,
            CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        };

        subscriber.Rentals.Add(rental);
        _context.SaveChanges();

        return Ok();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GetCopyDetails(SearchFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var copy = _context.BookCopies
            .Include(c => c.Book)
            .SingleOrDefault(c => c.SerialNumber.ToString() == model.Value && !c.IsDeleted && !c.Book!.IsDeleted);

        if (copy is null)
            return NotFound(Errors.InvalidSerialNumber);

        if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
            return BadRequest(Errors.NotAvilableRental);

        //check that copy is not in rental
        var copyIsInRental = _context.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);

        if (copyIsInRental)
            return BadRequest(Errors.CopyIsInRental);

        var viewModel = _mapper.Map<BookCopyViewModel>(copy);

        return PartialView("_CopyDetails", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAsDeleted(int rentalId)
    {
        var rental = _context.Rentals
            .Include(r => r.RentalCopies)
            .FirstOrDefault(r => r.Id == rentalId && !r.IsDeleted);

        if (rental is null)
            return NotFound();

        if (rental.CreatedOn.Date != DateTime.Today)
            return BadRequest(Errors.CannotCancelRental);


        rental.IsDeleted = true;
        
        rental.LastUpdatedOn = DateTime.Today;
        rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;



        _context.SaveChanges();
        return Ok();
    }



    private (string errorMessage, int? maxAllowedCopies) ValidateSubscriber(Subscriber subscriber)
    {
        if (subscriber.IsBlackListed)
            return (errorMessage: Errors.BlackListedSubscriber, maxAllowedCopies: null);

        if (subscriber.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalsConfigurations.RentalDuration))
            return (errorMessage: Errors.InactiveSubscriber, maxAllowedCopies: null);

        var currentRentals = subscriber.Rentals.SelectMany(r => r.RentalCopies).Count(c => !c.ReturnDate.HasValue);

        var availableCopiesCount = (int)RentalsConfigurations.MaxAllowedCopies - currentRentals;

        if (availableCopiesCount.Equals(0))
            return (errorMessage: Errors.MaxCopiesReached, maxAllowedCopies: null);

        return (errorMessage: string.Empty, maxAllowedCopies: availableCopiesCount);
    }
}
