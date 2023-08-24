using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Data;
using CineInfo_API.Models;
using CineInfo_API.Utilities;
using Microsoft.AspNetCore.Mvc;
using CineInfo_API.Validators;
using FluentValidation.Results;

namespace CineInfo_API.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase {
    private CineInfoContext _dbContext;
    private IMapper _mapper;
    private FindById<Session> _FindSessionById;
    private ListErrors _ListErrors;
    private Validation<InputSessionDTO> _Validation;

    public SessionController(CineInfoContext dbContext, IMapper mapper) {
        _dbContext = dbContext;
        _mapper = mapper;
        _FindSessionById = new FindById<Session>(_dbContext);
        _ListErrors = new ListErrors();
        _Validation = new Validation<InputSessionDTO>(new SessionValidator());
    }

    [HttpPost]
    public ActionResult AddSession([FromBody] InputSessionDTO sessionDTO) {
        ValidationResult result = _Validation.Validate(sessionDTO);
        if (result.IsValid) {
            Movie? movie = _dbContext.Movies.Find(sessionDTO.MovieId);
            if (movie == null)
                return NotFound("Filme não encontrado");
            Cinema? Cine = _dbContext.Cinemas.Find(sessionDTO.CinemaId);
            if (Cine == null)
                return NotFound("Filme não encontrado");

            Session session = _mapper.Map<Session>(sessionDTO);
            //Adiciona ao banco de dados

            ReadSessionDTO returnSession = _mapper.Map<ReadSessionDTO>(session);
            //return CreatedAtAction(
            //    nameof(GetCinemaById),
            //    new { id = returnCinema.Id },
            //    returnCinema
            //);
            return Created($"localhost/session/{returnSession.Id}", returnSession);
        }
        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }
}
