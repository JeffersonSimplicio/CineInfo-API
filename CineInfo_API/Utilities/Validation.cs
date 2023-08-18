using FluentValidation;
using FluentValidation.Results;

namespace CineInfo_API.Utilities;

public class Validation<T> {
    private readonly AbstractValidator<T> _validator;
    public Validation(AbstractValidator<T> validator) {
        _validator = validator;
    }

    public ValidationResult Validate(T dataForValidation) {
        ValidationResult result = _validator.Validate(dataForValidation);
        return result;
    }
}
