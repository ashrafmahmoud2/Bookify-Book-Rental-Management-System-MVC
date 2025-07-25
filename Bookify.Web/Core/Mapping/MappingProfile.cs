﻿using Bookify.Web.Core.ViewModel.Rental;
using Bookify.Web.Core.ViewModel.Serach;
using Bookify.Web.Core.ViewModel.Subscriber;

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


        //Books
        CreateMap<BookFormViewModel, Book>()
            .ReverseMap()
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<Book, BookViewModel>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
            .ForMember(dest => dest.Categories,
                opt => opt.MapFrom(src => src.Categories.Select(c => c.Category!.Name).ToList()));



            //BookCopy
             CreateMap<BookCopy, BookCopyViewModel>()
            .ForMember(dest => dest.BookTitle,opt => opt.MapFrom(src => src.Book!.Title))
            .ForMember(dest => dest.BookId,opt => opt.MapFrom(src => src.Book!.Id))
            .ForMember(dest => dest.BookThumbnailUrl,opt => opt.MapFrom(src => src.Book!.ImageThumbnailUrl)); 

        CreateMap<BookCopy, BookCopyFormViewModel>();


        //Users
        CreateMap<ApplicationUser, UserViewModel>();
        CreateMap<UserFormViewModel, ApplicationUser>()
            .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
            .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
            .ReverseMap();


        //Governrate && Areas
        CreateMap<Governorate, SelectListItem>()
        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        CreateMap<Area, SelectListItem>()
     .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));



        //Subscribers
        CreateMap<SubscriberFormViewModel, Subscriber>().ReverseMap();
        CreateMap<SubscriberViewModel, Subscriber>().ReverseMap();
        CreateMap<SubscriberSearchResultViewModel, Subscriber>().ReverseMap()
             .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        CreateMap<SubscriptionViewModel, Subscription>().ReverseMap();


        //Rental
        CreateMap<Rental, RentalViewModel>().ReverseMap();
        CreateMap<RentalCopy, RentalCopyViewModel>().ReverseMap();
        CreateMap<RentalCopy, CopyHistoryViewModel>()
            .ForMember(dest => dest.SubscriberMobile, opt => opt.MapFrom(src => src.Rental!.Subscriber!.PhoneNumber))
            .ForMember(dest => dest.SubscriberName, opt => opt.MapFrom(src => $"{src.Rental!.Subscriber!.LastName} {src.Rental!.Subscriber!.LastName}"));


        //search
        CreateMap<Book, SearchBookViewModel>().ReverseMap();




    }
}
