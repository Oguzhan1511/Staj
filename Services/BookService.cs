using AutoMapper;
using kitap.Core.Results;
using kitap.Core.UnitOfWork;
using kitap.Dtos;
using kitap.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kitap.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string BooksCacheKey = "BooksList";

        public BookService(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
        {
            _uow = uow;
            _mapper = mapper;
            _cache = cache;
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

            // Eğer filtreleme yoksa cache'den getirmeyi dene
            if (string.IsNullOrEmpty(searchTerm) && !categoryId.HasValue && !authorId.HasValue)
            {
                if (_cache.TryGetValue(BooksCacheKey, out IEnumerable<BooksDto>? cachedBooks))
                {
                    return new SuccessDataResult<IEnumerable<BooksDto>>(cachedBooks!.Skip((pageNumber - 1) * pageSize).Take(pageSize), "Veriler önbellekten getirildi.");
                }
            }

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

            var books = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<BooksDto>>(books);

            // Filtresiz ana listeyi cache'e atalım (10 dakika süreli)
            if (string.IsNullOrEmpty(searchTerm) && !categoryId.HasValue && !authorId.HasValue)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                
                _cache.Set(BooksCacheKey, dtos, cacheOptions);
            }

            var pagedData = dtos.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new SuccessDataResult<IEnumerable<BooksDto>>(pagedData, "Kitaplar başarıyla listelendi.");
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
            
            _cache.Remove(BooksCacheKey); // Veri değiştiği için cache'i temizle

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

            _cache.Remove(BooksCacheKey); // Cache invalidation

            return new SuccessResult("Kitap başarıyla güncellendi.");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var book = await _uow.Repository<Book>().GetByIdAsync(id);
            if (book == null) return new ErrorResult("Silinecek kitap bulunamadı.");

            _uow.Repository<Book>().Delete(book);
            await _uow.SaveChangesAsync();

            _cache.Remove(BooksCacheKey); // Cache invalidation

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
