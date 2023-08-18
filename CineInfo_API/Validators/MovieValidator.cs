using FluentValidation;
using CineInfo_API.Interfaces;

namespace CineInfo_API.Validators;

public class MovieValidator : AbstractValidator<IMovie> {
    public MovieValidator() {
        RuleFor(movie => movie.Title)
            .NotEmpty().WithMessage("O título do filme é obrigatório.")
            .Length(3, 100).WithMessage("O título deve ter entre 3 e 100 caracteres.");

        RuleFor(movie => movie.ReleaseYear)
            .InclusiveBetween(1850, DateTime.Now.Year).WithMessage("O ano de lançamento do filme deve estar entre 1850 e o ano atual.");

        RuleFor(movie => movie.Duration)
            .NotEmpty().WithMessage("A duração do filme é obrigatório.")
            .InclusiveBetween(70, 600).WithMessage("A duração do filme deve estar entre 70 e 600 minutos.");
    }
}
