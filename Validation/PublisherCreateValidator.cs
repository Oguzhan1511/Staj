using FluentValidation;
using kitap.Dtos;

namespace kitap.Validation
{
    public class PublisherCreateValidator : AbstractValidator<PublisherCreateDto>
    {
        public PublisherCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Yayınevi adı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Yayınevi adı en fazla 100 karakter olabilir.");
        }
    }
}
