using FluentValidation.Results;

namespace CineInfo_API.Utilities; 
public class ListErrors {
    public List<string> Generate(ValidationResult result) {
        List<string> errors = new List<string>(); 
        foreach (var failure in result.Errors) {
            errors.Add($"Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
        }
        return errors;
    }
}
