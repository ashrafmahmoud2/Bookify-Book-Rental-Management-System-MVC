namespace Bookify.Web.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Category
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryFormViewModel, Category>().ReverseMap();
        CreateMap<Category, SelectListItem>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));


        //Author
        CreateMap<Author, AuthorViewModel>();
        CreateMap<AuthorFormViewModel, Author>().ReverseMap();
        CreateMap<Author, SelectListItem>()
           .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));


        //Book
        CreateMap<Book, BookViewModel>()
                 .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Category.Name).ToList()));
        CreateMap<Book, BookDetailsViewModel>();
        CreateMap<BookFormViewModel, Book>()
            .ReverseMap()
                        .ForMember(dest => dest.Categories, opt => opt.Ignore());

    }
}
