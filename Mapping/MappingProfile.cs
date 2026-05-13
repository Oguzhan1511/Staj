using AutoMapper;
using kitap.Dtos;
using kitap.Models;

namespace kitap.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book mappings
            CreateMap<Book, BooksDto>().ReverseMap();
            CreateMap<BooksCreateDto, Book>();

            // Author mappings
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<AuthorCreateDto, Author>();

            // Publisher mappings
            CreateMap<Publisher, PublisherDto>().ReverseMap();
            CreateMap<PublisherCreateDto, Publisher>();

            // Category mappings
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryCreateDto, Category>();

            // Borrowing mappings
            CreateMap<Borrowing, BorrowingDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
            CreateMap<BorrowCreateDto, Borrowing>()
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => DateTime.UtcNow.AddDays(src.DaysToBorrow)));

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
            CreateMap<ReviewCreateDto, Review>();
        }
    }
}
