using FluentValidation;
using kitap.Dtos;

namespace kitap.Validation
{
    public class BookCreateValidator : AbstractValidator<BooksCreateDto>
    {
        public BookCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Kitap adı boş bırakılamaz.")
                .MaximumLength(200).WithMessage("Kitap adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Yazar alanı boş bırakılamaz.");

            RuleFor(x => x.PublisherId)
                .NotEmpty().WithMessage("Yayınevi alanı boş bırakılamaz.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori alanı boş bırakılamaz.");

            RuleFor(x => x.PublisherYear)
                .InclusiveBetween(1000, 2100).WithMessage("Geçerli bir yayın yılı giriniz.");
        }
    }
}
