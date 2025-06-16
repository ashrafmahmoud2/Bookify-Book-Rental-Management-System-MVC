namespace Bookify.Web.Core.ViewModel.Subscriber;



public class SubscriberFormViewModel
{
    public int Id { get; set; }


    [MaxLength(50, ErrorMessage = Errors.MaxLength), Display(Name = "FirstName"),
        RegularExpression(RegexPatterns.NumbersAndChrOnly_ArEng, ErrorMessage = Errors.OnlyNumbersAndLetters)]
    public string FirstName { get; set; } = null!;

    [MaxLength(50, ErrorMessage = Errors.MaxLength), Display(Name = "LastName"),
      RegularExpression(RegexPatterns.NumbersAndChrOnly_ArEng, ErrorMessage = Errors.OnlyNumbersAndLetters)]
    public string LastName { get; set; } = null!;


    [Required(ErrorMessage = Errors.RequiredField)]
    [StringLength(14, MinimumLength = 14, ErrorMessage = Errors.MaxMinLength)]
    [RegularExpression(RegexPatterns.NationalID, ErrorMessage = Errors.OnlyNumbers)]
    public string NationalId { get; set; } = null!;


    public IFormFile? Image { get; set; } = null!;


    [Url(ErrorMessage = Errors.InvalidUrl)]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = Errors.InvalidUrl)]
    public string? ImageThumbnailUrl { get; set; }

    public string? ImagePublicId { get; set; } // This is the image ID in the cloud, used for deletion or updates.  




    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } 

    [MaxLength(200, ErrorMessage = Errors.MaxLength), EmailAddress]
    //[Remote("AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
    public string Email { get; set; } = null!;

    [Phone]
    [Display(Name = "Phone number"), MaxLength(11, ErrorMessage = Errors.MaxLength),
                RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
    public string PhoneNumber { get; set; } = null!;


    [Required]
    [MaxLength(750, ErrorMessage = Errors.MaxLength)]
    public string Address { get; set; } = null!;


    public bool HasWhatsApp { get; set; } = true;

    public IEnumerable<Governorate> Governorates { get; set; } = new List<Governorate>();
    public IEnumerable<Area> Areas { get; set; } = new List<Area>();


    public int SelectedGovernorate { get; set; }

    public int SelectedArea { get; set; }


}
