using Moq;
using Xunit;
using kitap.Services;
using kitap.Core.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using kitap.Models;
using kitap.Dtos;
using System.Threading.Tasks;
using kitap.Core.Results;

namespace kitap.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockCache = new Mock<IMemoryCache>();
            _bookService = new BookService(_mockUow.Object, _mockMapper.Object, _mockCache.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenBookExists_ReturnsSuccess()
        {
            // Arrange
            var bookId = 1;
            var book = new Book { Id = bookId, Title = "Test Kitabı" };
            var bookDto = new BooksDto { Id = bookId, Title = "Test Kitabı" };

            _mockUow.Setup(x => x.Repository<Book>().GetByIdAsync(bookId))
                    .ReturnsAsync(book);
            
            _mockMapper.Setup(x => x.Map<BooksDto>(book))
                       .Returns(bookDto);

            // Act
            var result = await _bookService.GetByIdAsync(bookId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Test Kitabı", result.Data.Title);
            _mockUow.Verify(x => x.Repository<Book>().GetByIdAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenBookDoesNotExist_ReturnsError()
        {
            // Arrange
            var bookId = 99;
            _mockUow.Setup(x => x.Repository<Book>().GetByIdAsync(bookId))
                    .ReturnsAsync((Book)null!);

            // Act
            var result = await _bookService.GetByIdAsync(bookId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Kitap bulunamadı.", result.Message);
        }
    }
}
