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
