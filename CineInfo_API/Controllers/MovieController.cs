using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using CineInfo_API.Data;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Interfaces;
using CineInfo_API.Models;
using CineInfo_API.Validators;

namespace CineInfo_API.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase {
    private MovieContext _context;
    private IMapper _mapper;

    public MovieController(MovieContext context, IMapper mapper) {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public ActionResult AddMovie([FromBody] CreateMovieDTO movieDTO) {
        ValidationResult result = _Validation(movieDTO);

        if (result.IsValid) {
            Movie movie = _mapper.Map<Movie>(movieDTO);
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
        }

        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    [HttpGet]
    public ActionResult<List<ReadMovieDTO>> GetMoviesPagination([FromQuery] int skip = 0, int take = 50) {
        IQueryable<Movie> movies = _context.Movies.Skip(skip).Take(take);

        List<ReadMovieDTO> readMovieDTOs = movies.AsEnumerable()
            .Select(movie => _mapper.Map<ReadMovieDTO>(movie))
            .ToList();
        return Ok(readMovieDTOs);
    }

    [HttpGet("{id}")]
    public ActionResult GetMovieById(int id) {
        Movie? movie = _FindMovieById(id);
        if (movie == null) {
            return NotFound($"O filme com ID: {id}, não foi encontrado.");
        }

        ReadMovieDTO movieDTO = _mapper.Map<ReadMovieDTO>(movie);
        return Ok(movieDTO);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateMovie(int id, [FromBody] UpdateMovieDTO movieDTO) {
        Movie? movie = _FindMovieById(id);
        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        ValidationResult result = _Validation(movieDTO);

        if (result.IsValid) {
            _mapper.Map(movieDTO, movie);
            _context.SaveChanges();
            return NoContent();
        }
        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    [HttpPatch("{id}")]
    public ActionResult UpdatePatchMovie(int id, [FromBody] JsonPatchDocument<UpdateMovieDTO> patchMovie) {
        Movie? movie = _FindMovieById(id);
        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        UpdateMovieDTO movieForUpdate = _mapper.Map<UpdateMovieDTO>(movie);

        patchMovie.ApplyTo(movieForUpdate);

        ValidationResult result = _Validation(movieForUpdate);

        if (result.IsValid) {
            _mapper.Map(movieForUpdate, movie);
            _context.SaveChanges();
            return NoContent();
        }

        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteMovie(int id) {
        Movie? movie = _FindMovieById(id);

        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        _context.Movies.Remove(movie);
        _context.SaveChanges();
        return NoContent();
    }

    private Movie? _FindMovieById(int id) {
        Movie? movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);
        return movie;
    }

    private ValidationResult _Validation(IMovie movieForValidation) {
        var validator = new MovieValidator();
        ValidationResult result = validator.Validate(movieForValidation);
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
