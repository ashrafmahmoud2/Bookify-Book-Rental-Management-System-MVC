using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Web.Core.Models;

//[Index(nameof(FullName), IsUnique = true)]
public class Subscriber : BaseModel
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    //public string FullName => $"{FirstName} {LastName}";

    public string NationalId { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? ImageThumbnailUrl { get; set; }

    public string? ImagePublicId { get; set; } // This is the image ID in the cloud, used for deletion or updates.  

    public DateTime DateOfBirth { get; set; }

    [EmailAddress]
    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool HasWhatsApp { get; set; } = true;

    public int SelectedGovernorate { get; set; }

    public int SelectedArea { get; set; }
}




public class Governorate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Area> Areas { get; set; } = new HashSet<Area>(); 
}

public class Area
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int GovernorateId { get; set; }

    public Governorate Governorate { get; set; } 
}

