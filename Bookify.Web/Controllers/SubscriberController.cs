
using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel.Subscriber;
using Bookify.Web.Settings;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;

namespace Bookify.Web.Controllers;
public class SubscribersController : Controller
{


    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly Cloudinary _cloudinary;

    private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
    private int _maxAllowedSize = 2097152;

    public SubscribersController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> cloudinary)
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
        return View();
    }

    public async Task<IActionResult> Create()
    {
        var model = new SubscriberFormViewModel
        {
            Governorates = await _context.Governorates.AsNoTracking().ToListAsync(),
            Areas = await _context.Areas.AsNoTracking().ToListAsync()
        };
        return View("Form", model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubscriberFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

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


        subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        _context.Add(subscriber);
        await _context.SaveChangesAsync();

        return Ok();
    }





    [HttpGet]
    public async Task<IActionResult> Edit(int SubscriberId)
    {

        SubscriberId = 5;
        var subscriber = await _context.Subscribers.AsNoTracking()
                            .SingleOrDefaultAsync(s => s.Id == SubscriberId);

        if (subscriber == null)
            return NotFound();

        var viewModel = _mapper.Map<SubscriberFormViewModel>(subscriber);

        // Load Governorates and Areas for dropdowns
        viewModel.Governorates = await _context.Governorates.AsNoTracking().ToListAsync();
        viewModel.Areas = await _context.Areas.AsNoTracking()
                          .Where(a => a.GovernorateId == viewModel.SelectedGovernorate) // Load areas for the selected governorate
                          .ToListAsync();

        return View("Form", viewModel);
    }



    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubscriberFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var subscriber = _mapper.Map<Subscriber>(model);
        subscriber.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        subscriber.LastUpdatedOn = DateTime.Now;
        _context.Update(subscriber);
        await _context.SaveChangesAsync();

        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> GetAreasByGovernorate(int governorateId)
    {
        var areas = await _context.Areas
                                  .Where(a => a.GovernorateId == governorateId)
                                  .Select(a => new { id = a.Id, name = a.Name })
                                  .ToListAsync();

        return Json(areas);
    }


    private string GetThumbnailUrl(string url)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("/upload/"))
            return url;

        return url.Replace("/upload/", "/upload/c_thumb,w_200,g_face/");
    }



}
