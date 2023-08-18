using CineInfo_API.Interfaces;
using FluentValidation;

namespace CineInfo_API.Validators; 
public class AddressValidatior : AbstractValidator<IAddress> {
    public AddressValidatior() {
        RuleFor(address => address.Neighborhood)
            .NotEmpty().WithMessage("O nome do bairro é obrigatorio.")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres.");
        RuleFor(address => address.Street)
            .NotEmpty().WithMessage("O nome da rua é obrigatorio.")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres.");
        RuleFor(address => address.Number)
            .InclusiveBetween(1, 5000).WithMessage("O número do endereço deve estar entre 1 e 5000");
    }
}
