using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel.Rental;
using Bookify.Web.Core.ViewModel.Subscriber;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bookify.Web.Controllers;
public class RentalsController : Controller
{

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
        //https://localhost:44360/Rentals/Create?sKey=CfDJ8EBE7_7Rq2BIm37SQMKUy0jC7SJmZ0ZuJVKrc7ruV92wxMTTaihgt6zcmYqzO9esLG8lPiyWO-TrVca0SyzCsXS93KBnu3Rdi6TJeDf-IHz6LqSi3-SCTp6zHQI09BgyYw

        var viewModel = new RentalFormViewModel
        {
            SubscriberKey = sKey
        };

        return View(viewModel);
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


        var viewModel = _mapper.Map<BookCopyViewModel>(copy);

        return PartialView("_CopyDetails", viewModel);
    }



}
