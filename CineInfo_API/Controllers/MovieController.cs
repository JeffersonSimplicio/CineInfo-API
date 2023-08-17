using AutoMapper;
using CineInfo_API.Data;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Interfaces;
using CineInfo_API.Models;
using CineInfo_API.Utilities;
using CineInfo_API.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CineInfo_API.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase {
    private CineInfoContext _dbContext;
    private IMapper _mapper;
    private FindById<Movie> _FindMovieById;

    public MovieController(CineInfoContext dbContext, IMapper mapper) {
        _dbContext = dbContext;
        _mapper = mapper;
        _FindMovieById = new FindById<Movie>(_dbContext);
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="movieDTO">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    /// <response code="400">Caso alguma imformação não esta dentro das regras</response>
    [HttpPost]
    public ActionResult AddMovie([FromBody] CreateMovieDTO movieDTO) {
        ValidationResult result = _Validation(movieDTO);

        if (result.IsValid) {
            Movie movie = _mapper.Map<Movie>(movieDTO);
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetMovieById), new { id = movie.Id }, movie);
        }

        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Retona filmes de forma paginada
    /// </summary>
    /// <param name="skip">Quantos filmes devem ser pulados</param>
    /// <param name="take">Quantos filmes devem ser retornados</param>
    /// <returns>ActionResult{List{ReadMovieDTO}}</returns>
    /// <response code="200">Caso a consulta tenha sido bem sucedida</response>
    [HttpGet]
    public ActionResult<List<ReadMovieDTO>> GetMoviesPagination([FromQuery] int skip = 0, int take = 50) {
        IQueryable<Movie> movies = _dbContext.Movies.Skip(skip).Take(take);

        List<ReadMovieDTO> readMovieDTOs = movies.AsEnumerable()
            .Select(movie => _mapper.Map<ReadMovieDTO>(movie))
            .ToList();
        return Ok(readMovieDTOs);
    }

    /// <summary>
    /// Retona o filme com o ID informando
    /// </summary>
    /// <param name="id">Identificador(id) do filme que deseja buscar</param>
    /// <returns>ActionResult{List{ReadMovieDTO}}</returns>
    /// <response code="200">Caso a busca seja bem sucedida</response>
    /// <response code="404">Caso nenhum filme seja encontrado com o ID informado</response>
    [HttpGet("{id}")]
    public ActionResult GetMovieById(int id) {
        Movie? movie = _FindMovieById.Find(id);
        if (movie == null) {
            return NotFound($"O filme com ID: {id}, não foi encontrado.");
        }

        ReadMovieDTO movieDTO = _mapper.Map<ReadMovieDTO>(movie);
        return Ok(movieDTO);
    }

    /// <summary>
    /// Atualiza um filme existente pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do filme que deseja atualizar</param>
    /// <param name="movieDTO">Objeto com os campos a serem atualizados do filme</param>
    /// <returns><see cref="ActionResult"/></returns>
    /// <response code="204">Caso a atualização seja bem sucedida</response>
    /// <response code="404">Caso nenhum filme seja encontrado com o ID informado</response>
    [HttpPut("{id}")]
    public ActionResult UpdateMovie(int id, [FromBody] UpdateMovieDTO movieDTO) {
        Movie? movie = _FindMovieById.Find(id);
        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        ValidationResult result = _Validation(movieDTO);

        if (result.IsValid) {
            _mapper.Map(movieDTO, movie);
            _dbContext.SaveChanges();
            return NoContent();
        }
        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Atualiza parcialmente um filme existente pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do filme que deseja atualizar parcialmente</param>
    /// <param name="patchMovie">JsonPatchDocument contendo as atualizações a serem aplicadas</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização parcial seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nas atualizações</response>
    /// <response code="404">Caso nenhum filme seja encontrado com o ID informado</response>
    [HttpPatch("{id}")]
    public ActionResult UpdatePatchMovie(int id, [FromBody] JsonPatchDocument<UpdateMovieDTO> patchMovie) {
        Movie? movie = _FindMovieById.Find(id);
        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        UpdateMovieDTO movieForUpdate = _mapper.Map<UpdateMovieDTO>(movie);

        patchMovie.ApplyTo(movieForUpdate);

        ValidationResult result = _Validation(movieForUpdate);

        if (result.IsValid) {
            _mapper.Map(movieForUpdate, movie);
            _dbContext.SaveChanges();
            return NoContent();
        }

        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Exclui um filme pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do filme que deseja excluir</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a exclusão seja bem sucedida</response>
    /// <response code="404">Caso nenhum filme seja encontrado com o ID informado</response>
    [HttpDelete("{id}")]
    public ActionResult DeleteMovie(int id) {
        Movie? movie = _FindMovieById.Find(id);

        if (movie == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        _dbContext.Movies.Remove(movie);
        _dbContext.SaveChanges();
        return NoContent();
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
