using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Movies_API.Models;
using Movies_API.Validators;

namespace Movies_API.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase {
    private static int currentId = -1;
    private static List<Movie> movies = new List<Movie>();

    [HttpPost]
    public ActionResult AddMovie([FromBody] Movie movie) {
        var validator = new MovieValidator();
        ValidationResult result = validator.Validate(movie);

        if (result.IsValid) {
            currentId++;
            movie.Id = currentId;
            movies.Add(movie);
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
        } else {
            List<string> errors = new List<string>();
            foreach (var failure in result.Errors) {
                errors.Add($"Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
            }
            return BadRequest(errors);
        }
    }
}
