using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Movies_API.Data;
using Movies_API.DTOs;
using Movies_API.Models;
using Movies_API.Validators;

namespace Movies_API.Controllers;

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
    public ActionResult AddMovie([FromBody] MovieDTO movieDTO) {
        ValidationResult result = this.Validation(movieDTO);

        if (result.IsValid) {
            Movie movie = _mapper.Map<Movie>(movieDTO);
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
        }
        List<string> errors = new List<string>();
        foreach (var failure in result.Errors) {
            errors.Add($"Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
        }
        return BadRequest(errors);
    }

    [HttpGet]
    public OkObjectResult GetMoviesPagination([FromQuery] int skip = 0, int take = 50) {
        return Ok(_context.Movies.Skip(skip).Take(take));
    }

    [HttpGet("{id}")]
    public ActionResult GetMovieById(int id) {
        Movie? movie = this.FindMovieById(id);
        if (movie != null) return Ok(movie);
        return NotFound($"O filme com ID: {id}, não foi encontrado.");
    }

    [HttpPut("{id}")]
    public ActionResult UpdateMovie(int id, [FromBody] MovieDTO movieDTO) {
        Movie? movie = this.FindMovieById(id);
        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        ValidationResult result = this.Validation(movieDTO);

        if (result.IsValid) {
            _mapper.Map(movieDTO, movie);
            _context.SaveChanges();
            return NoContent();
        }
        List<string> errors = new List<string>();
        foreach (var failure in result.Errors) {
            errors.Add($"Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
        }
        return BadRequest(errors);
    }

    [HttpPatch("{id}")]
    public ActionResult UpdatePatchMovie(int id, [FromBody] JsonPatchDocument<MovieDTO> patchMovie) {
        Movie? movie = this.FindMovieById(id);
        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        MovieDTO movieForUpdate = _mapper.Map<MovieDTO>(movie);

        patchMovie.ApplyTo(movieForUpdate);

        ValidationResult result = this.Validation(movieForUpdate);

        if (result.IsValid) {
            _mapper.Map(movieForUpdate, movie);
            _context.SaveChanges();
            return NoContent();
        }

        List<string> errors = new List<string>();
        foreach (var failure in result.Errors) {
            errors.Add($"Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
        }
        return BadRequest(errors);
    }

    private Movie? FindMovieById(int id) {
        Movie? movie = _context.Movies.FirstOrDefault(movie => movie.Id == id);
        return movie;
    }

    private ValidationResult Validation(MovieDTO movieForValidation) {
        var validator = new MovieValidator();
        ValidationResult result = validator.Validate(movieForValidation);
        return result;
    }
}
