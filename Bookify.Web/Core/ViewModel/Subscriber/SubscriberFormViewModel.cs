﻿using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel.Subscriber;

public class SubscriberFormViewModel
{
    //public int Id { get; set; }
    
    public string? Key { get; set; }

    [MaxLength(100), Display(Name = "First Name"),
        RegularExpression(RegexPatterns.DenySpecialCharacters, ErrorMessage = Errors.DenySpecialCharacters)]
    public string FirstName { get; set; } = null!;

    [MaxLength(100), Display(Name = "Last Name"),
        RegularExpression(RegexPatterns.DenySpecialCharacters, ErrorMessage = Errors.DenySpecialCharacters)]
    public string LastName { get; set; } = null!;

    [Display(Name = "Date Of Birth")]
    [AssertThat("DateOfBirth <= Today()", ErrorMessage = Errors.NotAllowFutureDates)]
    public DateTime DateOfBirth { get; set; } = DateTime.Now;

    [MaxLength(14), Display(Name = "National ID"),
        RegularExpression(RegexPatterns.NationalId, ErrorMessage = Errors.InvalidNationalId)]
    [Remote("AllowNationalId", null!, AdditionalFields = "Key", ErrorMessage = Errors.Duplicated)]
    public string NationalId { get; set; } = null!;

    [MaxLength(11), Display(Name = "Mobile Number"),
        RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]
    [Remote("AllowMobileNumber", null!, AdditionalFields = "Key", ErrorMessage = Errors.Duplicated)]
    public string PhoneNumber { get; set; } = null!;
    
    public bool HasWhatsApp { get; set; }

    [MaxLength(150), EmailAddress]
    [Remote("AllowEmail", null!, AdditionalFields = "Key", ErrorMessage = Errors.Duplicated)]
    public string Email { get; set; } = null!;

    [RequiredIf("Key == ''", ErrorMessage = Errors.EmptyImage)]
    public IFormFile? Image { get; set; }

    public string? ImagePublicId { get; set; } // This is the image ID in the cloud, used for deletion or updates.  


    [Display(Name = "Area")]
    public int AreaId { get; set; }

    public IEnumerable<SelectListItem>? Areas { get; set; } = new List<SelectListItem>();

    [Display(Name = "Governorate")]
    public int GovernorateId { get; set; }

    public IEnumerable<SelectListItem>? Governorates { get; set; }

    [MaxLength(500)]
    public string Address { get; set; } = null!;

    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
}
