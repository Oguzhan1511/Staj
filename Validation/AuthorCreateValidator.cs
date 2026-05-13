using FluentValidation;
using kitap.Dtos;

namespace kitap.Validation
{
    public class AuthorCreateValidator : AbstractValidator<AuthorCreateDto>
    {
        public AuthorCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Yazar adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Yazar adı en fazla 100 karakter olabilir.");
        }
    }
}
