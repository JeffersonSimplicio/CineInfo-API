using AutoMapper;
using CineInfo_API.Data;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Interfaces;
using CineInfo_API.Models;
using CineInfo_API.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CineInfo_API.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : Controller {
    private CineInfoContext _dbContext;
    private IMapper _mapper;

    public CinemaController(CineInfoContext dbContext, IMapper mapper) {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um novo cinema
    /// </summary>
    /// <param name="cinemaDTO">Objeto com os campos necessários para criação de um cinema</param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Caso a criação seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    [HttpPost]
    public ActionResult AddCinema([FromBody] CreateCinemaDTO cinemaDTO) {
        var result = _Validation(cinemaDTO);

        if (result.IsValid) {
            Cinema cine = _mapper.Map<Cinema>(cinemaDTO);
            _dbContext.Cinemas.Add(cine);
            _dbContext.SaveChanges();
            //return CreatedAtAction(nameof(GetCineById), new { id = cine.Id }, cine);
            return Created($"localhost/cinema/{cine.Id}", cine);
        }
        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    [HttpGet]
    public ActionResult<List<ReadMovieDTO>> GetCinemaPagination([FromQuery] int skip = 0, int take = 50) {
        IQueryable<Cinema> cines = _dbContext.Cinemas.Skip(skip).Take(take);

        List<ReadMovieDTO> readMovieDTOs = cines.AsEnumerable()
            .Select(movie => _mapper.Map<ReadMovieDTO>(movie))
            .ToList();
        return Ok(readMovieDTOs);
    }

    private ValidationResult _Validation(ICinema cineForValidation) {
        var validator = new CinemaValidator();
        ValidationResult result = validator.Validate(cineForValidation);
        return result;
    }

    private List<string> _ListErrors(ValidationResult result) {
        List<string> errors = new List<string>();
        foreach (var failure in result.Errors) {
            errors.Add($"Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
        }
        return errors;
    }
}
