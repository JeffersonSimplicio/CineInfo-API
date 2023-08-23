using FluentValidation;
using CineInfo_API.Data.DTOs;

namespace CineInfo_API.Validators; 
public class SessionValidator : AbstractValidator<InputSessionDTO> {
    public SessionValidator() {
        RuleFor(session => session.MovieId)
            .NotEmpty().WithMessage("O Id do filme é obrigatorio.")
            .GreaterThanOrEqualTo(1).WithMessage("O Id do filme deve ser maior ou igual a 1.");
        RuleFor(session => session.CinemaId)
            .NotEmpty().WithMessage("O Id do cinema é obrigatorio.")
            .GreaterThanOrEqualTo(1).WithMessage("O Id do cinema deve ser maior ou igual a 1.");
        RuleFor(session => session.StartTime)
            .NotEmpty().WithMessage("O horário de início da sessão é obrigatória.")
            .Must(date => date.Year >= DateTime.Now.Year).WithMessage("A data da sessão deve ser a partir do ano atual.")
            .Must(date => date.Hour >= 9 && date.Hour <= 23).WithMessage("O horário deve estar entre 9h e 23h.");
        RuleFor(session => session.TicketPrice)
            .NotEmpty().WithMessage("O valor do ingresso é obrigatorio.")
            .GreaterThan(0).WithMessage("O ingresso não pode ter o valor menor ou igual a zero.");
    }
}
