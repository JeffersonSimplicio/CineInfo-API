using FluentValidation;
using CineInfo_API.Interfaces;

namespace CineInfo_API.Validators;

public class CinemaValidator : AbstractValidator<ICinema> {
    public CinemaValidator() {
        RuleFor(cine => cine.Name)
            .NotEmpty().WithMessage("O nome do cinema é obrigatório.")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres.");

        RuleFor(cine => cine.NumberHalls)
            .InclusiveBetween(1, 50).WithMessage("Aquantidade de salas deve estar entre 1 e 50");
    }
}
