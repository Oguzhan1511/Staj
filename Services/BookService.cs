using AutoMapper;
using kitap.Core.Results;
using kitap.Core.UnitOfWork;
using kitap.Dtos;
using kitap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IDataResult<IEnumerable<BooksDto>>> GetAllAsync(
            string? searchTerm, 
            int? categoryId, 
            int? authorId, 
            string? sortBy, 
            int pageNumber, 
            int pageSize)
        {
            if (pageSize > 50) pageSize = 50;

            var query = _uow.Repository<Book>().GetQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(b => b.Title.ToLower().Contains(searchTerm.ToLower()));

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId.Value);

            if (authorId.HasValue)
                query = query.Where(b => b.AuthorId == authorId.Value);

            query = sortBy switch
            {
                "name" => query.OrderBy(b => b.Title),
                "name_desc" => query.OrderByDescending(b => b.Title),
                "year" => query.OrderBy(b => b.PublisherYear),
                "year_desc" => query.OrderByDescending(b => b.PublisherYear),
                _ => query.OrderBy(b => b.Id)
            };

            var books = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<BooksDto>>(books);
            return new SuccessDataResult<IEnumerable<BooksDto>>(dtos, "Kitaplar başarıyla listelendi.");
        }

        public async Task<IDataResult<BooksDto>> GetByIdAsync(int id)
        {
            var book = await _uow.Repository<Book>().GetByIdAsync(id);
            if (book == null) return new ErrorDataResult<BooksDto>("Kitap bulunamadı.");

            var dto = _mapper.Map<BooksDto>(book);
            return new SuccessDataResult<BooksDto>(dto);
        }

        public async Task<IDataResult<BooksDto>> CreateAsync(BooksCreateDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            await _uow.Repository<Book>().AddAsync(book);
            await _uow.SaveChangesAsync();

            var dto = _mapper.Map<BooksDto>(book);
            return new SuccessDataResult<BooksDto>(dto, "Kitap başarıyla eklendi.");
        }

        public async Task<IResult> UpdateAsync(int id, BooksCreateDto bookDto)
        {
            var book = await _uow.Repository<Book>().GetByIdAsync(id);
            if (book == null) return new ErrorResult("Güncellenecek kitap bulunamadı.");

            _mapper.Map(bookDto, book);
            _uow.Repository<Book>().Update(book);
            await _uow.SaveChangesAsync();

            return new SuccessResult("Kitap başarıyla güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var book = await _uow.Repository<Book>().GetByIdAsync(id);
            if (book == null) return new ErrorResult("Silinecek kitap bulunamadı.");

            _uow.Repository<Book>().Delete(book);
            await _uow.SaveChangesAsync();

            return new SuccessResult("Kitap başarıyla silindi.");
        }

        public async Task<IResult> UpdateImageAsync(int id, string imageUrl)
        {
            var book = await _uow.Repository<Book>().GetByIdAsync(id);
            if (book == null) return new ErrorResult("Kitap bulunamadı.");

            book.ImageUrl = imageUrl;
            _uow.Repository<Book>().Update(book);
            await _uow.SaveChangesAsync();

            return new SuccessResult("Kitap resmi güncellendi.");
        }
    }
}
