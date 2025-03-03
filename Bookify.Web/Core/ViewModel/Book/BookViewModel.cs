namespace Bookify.Web.Core.ViewModel.Book;


public record BookViewModel(
    int Id,
    string Name,
    string ImageUrl,
    string AuthorName,
    string Description,
    string Publisher,
    DateTime PublishingDate,
    string Hall,
    bool IsRental,
    string CategoryName,
    bool IsDeleted
);


//public class BookViewModel
//{

//    public int Id { get; set; }

//    public string Name { get; set; } = null!;

//    public string ImageUrl { get; set; } = null!;

//    public string AuthorName { get; set; } = null!;

//    public string Description { get; set; } = null!;

//    public string Publisher { get; set; } = null!;

//    public DateTime PublishingDate { get; set; }

//    public string Hall { get; set; } = null!;

//    public bool IsRental { get; set; }


//    public bool IsDeleted { get; set; }

//    public string CategoryName { get; set; } = null!;


//}
