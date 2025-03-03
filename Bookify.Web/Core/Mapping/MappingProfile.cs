
using Bookify.Web.Core.ViewModel.Author;
using Bookify.Web.Core.ViewModel.Category;

namespace Bookify.Web.Core.Mapping;

public class MappingProfile :Profile
{
    public MappingProfile()
    {
        //Category
        CreateMap<Category,CategoryViewModel>();
        CreateMap<CategoryFormViewModel, Category>().ReverseMap();


        //Author
        CreateMap<Author, AuthorViewModel>();
        CreateMap<AuthorFormViewModel, Author>().ReverseMap();


        //Book
        CreateMap<Book, BookViewModel>();
        CreateMap<BookFormViewModel, Book>().ReverseMap();
    }
}
