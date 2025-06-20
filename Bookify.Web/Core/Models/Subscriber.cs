using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Web.Core.Models;

[Index(nameof(NationalId), IsUnique = true)]
[Index(nameof(PhoneNumber), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Subscriber : BaseModel
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string NationalId { get; set; } = null!;

    [MaxLength(15)]
    public string PhoneNumber { get; set; } = null!;

    public bool HasWhatsApp { get; set; }

    [MaxLength(150)]
    public string Email { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string ImageThumbnailUrl { get; set; } = null!;

    public string? ImagePublicId { get; set; } // This is the image ID in the cloud, used for deletion or updates.  

    public int AreaId { get; set; }

    public Area? Area { get; set; }

    public int GovernorateId { get; set; }

    public Governorate? Governorate { get; set; }

    [MaxLength(500)]
    public string Address { get; set; } = null!;

    public bool IsBlackListed { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

}
