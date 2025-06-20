using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel.Subscriber;
using Bookify.Web.Services;
using Bookify.Web.Settings;
using CloudinaryDotNet;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace Bookify.Web.Controllers;
public class SubscribersController : Controller
{

    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly Cloudinary _cloudinary;
    private readonly IDataProtector _dataProtector;
    private readonly IEmailBodyBuilder _emailBodyBuilder ;
    private readonly IEmailSender _emailSender;


    private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
    private int _maxAllowedSize = 2097152;

    public SubscribersController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment,
        IOptions<CloudinarySettings> cloudinary, IDataProtectionProvider dataProtector, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender)
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
        _dataProtector = dataProtector.CreateProtector("MySecuerKey");
        _emailBodyBuilder = emailBodyBuilder;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Search(SearchFormViewModel model)
    {
        //model.Value = "30308161201733";
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var subscriber = _context.Subscribers
                        .SingleOrDefault(s =>
                                s.Email == model.Value
                            || s.NationalId == model.Value
                            || s.PhoneNumber == model.Value);

        var viewModel = _mapper.Map<SubscriberSearchResultViewModel>(subscriber);

        if (subscriber is not null)
            viewModel.Key = _dataProtector.Protect(subscriber.Id.ToString());


        return PartialView("_Result", viewModel);
    }


    public IActionResult Details(string id)
    {

        var subsciberId = int.Parse(_dataProtector.Unprotect(id));

        var subscriber = _context.Subscribers
            .Include(s => s.Governorate)
            .Include(s => s.Area)
            .Include(s => s.Subscriptions)
            .SingleOrDefault(s => s.Id == subsciberId);

        if (subscriber is null)
            return NotFound();

        var viewModel = _mapper.Map<SubscriberViewModel>(subscriber);
        viewModel.Key = id;

        return View(viewModel);
    }


    public IActionResult Create()
    {
        var viewModel = PopulateViewModel();
        return View("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubscriberFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel(model));

        var subscriber = _mapper.Map<Subscriber>(model);


        if (model.Image is not null)
        {
            var extension = Path.GetExtension(model.Image.FileName);

            if (!_allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                return View("Form");
            }

            if (model.Image.Length > _maxAllowedSize)
            {
                ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                return View("Form");
            }

            var imageName = $"{Guid.NewGuid()}{extension}";


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
                return View("Form");
            }

            subscriber.ImageUrl = Result.SecureUrl.ToString();
            subscriber.ImageThumbnailUrl = GetThumbnailUrl(subscriber.ImageUrl);
            subscriber.ImagePublicId = Result.PublicId;
        }

        Subscription subscription = new()
        {
            CreatedById = subscriber.CreatedById,
            CreatedOn = subscriber.CreatedOn,
            Subscriber = subscriber,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };


        subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _context.Add(subscriber);
        await _context.SaveChangesAsync();


        var subscriberId = _dataProtector.Protect(subscriber.Id.ToString());
        return RedirectToAction(nameof(Details), new { id = subscriberId });
    }

    public IActionResult Edit(string id)
    {
         var subscriberId = int.Parse(_dataProtector.Unprotect(id));

        var subscriber = _context.Subscribers.Find(subscriberId);

        if (subscriber is null)
            return NotFound();

        var model = _mapper.Map<SubscriberFormViewModel>(subscriber);
        var viewModel = PopulateViewModel(model);
        viewModel.Key = id;

        return View("Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubscriberFormViewModel model)
    {


        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel(model));

        var subscriberId = int.Parse(_dataProtector.Unprotect(model.Key!));


        var subscriber = await _context.Subscribers.FindAsync(subscriberId); // Made this query async to avoid blocking

        if (subscriber is null)
            return NotFound();


        string imagePublicId = null;

        if (model.Image is not null)
        {
            if (!string.IsNullOrEmpty(subscriber.ImageUrl))
                await _cloudinary.DeleteResourcesAsync(subscriber.ImagePublicId);


            var extension = Path.GetExtension(model.Image.FileName);

            if (!_allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                return View("Form");
            }

            if (model.Image.Length > _maxAllowedSize)
            {
                ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                return View("Form");
            }

            var imageName = $"{Guid.NewGuid()}{extension}";
            using var stream = model.Image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageName, stream),
                UseFilename = true,
                Folder = "books",
                PublicId = Guid.NewGuid().ToString(),
                Overwrite = false,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ModelState.AddModelError(nameof(model.Image), "Image upload failed.");
                return View("Form");
            }

            model.ImageUrl = result.SecureUrl.ToString();
            imagePublicId = result.PublicId;
        }
        else
        {
            model.ImageUrl = subscriber.ImageUrl;
            model.ImageThumbnailUrl = subscriber.ImageThumbnailUrl;
        }




        _mapper.Map(model, subscriber);
        subscriber.ImageThumbnailUrl = GetThumbnailUrl(subscriber.ImageUrl);
        subscriber.ImagePublicId = imagePublicId;
        subscriber.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        subscriber.LastUpdatedOn = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = model.Key });
         
    }

    private string GetThumbnailUrl(string url)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("/upload/"))
            return url;

        return url.Replace("/upload/", "/upload/c_thumb,w_200,g_face/");
    }


    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult RenewSubscription(string sKey)
    //{
    //    var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));


    //    var subscriber = _context.Subscribers
    //        .Include(s => s.Subscriptions)
    //        .SingleOrDefault(s => s.Id == subscriberId);

    //    if (subscriber is null)
    //        return NotFound();


    //    if (subscriber.IsBlackListed)
    //        BadRequest();

    //    var lastSubscription = subscriber.Subscriptions.Last();
    //    var startData = lastSubscription.EndDate < DateTime.Today
    //        ? DateTime.Today 
    //        : lastSubscription.EndDate.AddDays(1) ;


    //    Subscription newSubscription = new ()
    //    {
    //        CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
    //        CreatedOn = DateTime.Now,
    //        StartDate = startData,
    //        EndDate = startData.AddYears(1),


    //    };

    //    _context.Subscriptions.Add(newSubscription);

    //    _context.SaveChanges();

    //    var viewModel = _mapper.Map<SubscriptionViewModel>(newSubscription);

    //    return PartialView("_SubscriptionRow", viewModel);
    //}


    [AjaxOnly]
    public async Task<IActionResult> GetAreasByGovernorate(int governorateId)
    {
        var areas = await _context.Areas
                                  .Where(a => a.GovernorateId == governorateId && !a.IsDeleted)
                                  .Select(a => new { id = a.Id, name = a.Name })
                                  .ToListAsync();

        return Json(areas);
    }

    public IActionResult AllowNationalId(SubscriberFormViewModel model)
    {
        var subscriberId = 0;
        if (!string.IsNullOrEmpty(model.Key))
            subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

        var Subscriber = _context.Subscribers.SingleOrDefault(b => b.NationalId == model.NationalId);
        var isAllowed = Subscriber is null || Subscriber.Id.Equals(subscriberId);

        return Json(isAllowed);
    }

    public IActionResult AllowMobileNumber(SubscriberFormViewModel model)
    {
        var subscriberId = 0;
        if (!string.IsNullOrEmpty(model.Key))
            subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

        var Subscriber = _context.Subscribers.SingleOrDefault(b => b.PhoneNumber == model.PhoneNumber);
        var isAllowed = Subscriber is null || Subscriber.Id.Equals(subscriberId);

        return Json(isAllowed);
    }

    public IActionResult AllowEmail(SubscriberFormViewModel model)
    {
        var subscriberId = 0;
        if (!string.IsNullOrEmpty(model.Key))
            subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

        var Subscriber = _context.Subscribers.SingleOrDefault(b => b.Email == model.Email);
        var isAllowed = Subscriber is null || Subscriber.Id.Equals(subscriberId);

        return Json(isAllowed);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RenewSubscription(string sKey)
    {
        var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));

        var subscriber = _context.Subscribers
                                    .Include(s => s.Subscriptions)
                                    .SingleOrDefault(s => s.Id == subscriberId);

        if (subscriber is null)
            return NotFound();

        if (subscriber.IsBlackListed)
            return BadRequest();

        var lastSubscription = subscriber.Subscriptions.Last();

        var startDate = lastSubscription.EndDate < DateTime.Today
                        ? DateTime.Today
                        : lastSubscription.EndDate.AddDays(1);

        Subscription newSubscription = new()
        {
            CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
            CreatedOn = DateTime.Now,
            StartDate = startDate,
            EndDate = startDate.AddYears(1)
        };

        subscriber.Subscriptions.Add(newSubscription);

        _context.SaveChanges();

        //Send email and WhatsApp Message
        var placeholders = new Dictionary<string, string>()
            {
                { "imageUrl", "https://res.cloudinary.com/devcreed/image/upload/v1668739431/icon-positive-vote-2_jcxdww.svg" },
                { "header", $"Hello {subscriber.FirstName}," },
                { "body", $"your subscription has been renewed through {newSubscription.EndDate.ToString("d MMM, yyyy")} 🎉🎉" }
            };

        var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(
            subscriber.Email,
            "Bookify Subscription Renewal", body));

        //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(
        //    subscriber.Email,
        //    "Bookify Subscription Renewal", body), TimeSpan.FromMinutes(1));

        //if (subscriber.HasWhatsApp)
        //{
        //    var components = new List<WhatsAppComponent>()
        //        {
        //            new WhatsAppComponent
        //            {
        //                Type = "body",
        //                Parameters = new List<object>()
        //                {
        //                    new WhatsAppTextParameter { Text = subscriber.FirstName },
        //                    new WhatsAppTextParameter { Text = newSubscription.EndDate.ToString("d MMM, yyyy") },
        //                }
        //            }
        //        };

        //    var mobileNumber = _webHostEnvironment.IsDevelopment() ? "Add Your Number" : subscriber.MobileNumber;

        //    //Change 2 with your country code
        //    BackgroundJob.Enqueue(() => _whatsAppClient
        //        .SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English,
        //        WhatsAppTemplates.SubscriptionRenew, components));
        //}

        var viewModel = _mapper.Map<SubscriptionViewModel>(newSubscription);

        return PartialView("_SubscriptionRow", viewModel);
    }


    private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model = null)
    {
        SubscriberFormViewModel viewModel = model is null ? new SubscriberFormViewModel() : model;

        var governorates = _context.Governorates.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();
        viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorates);

        if (model?.GovernorateId > 0)
        {
            var areas = _context.Areas
                .Where(a => a.GovernorateId == model.GovernorateId && !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ToList();

            viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
        }

        return viewModel;
    }

}
