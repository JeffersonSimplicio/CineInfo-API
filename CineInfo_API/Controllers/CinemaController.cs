 using AutoMapper;
using CineInfo_API.Data;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Interfaces;
using CineInfo_API.Models;
using CineInfo_API.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
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
            return CreatedAtAction(nameof(GetCinemaById), new { id = cine.Id }, cine);
        }
        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Obtém uma lista paginada de cinemas
    /// </summary>
    /// <param name="skip">Número de itens a serem ignorados (padrão: 0)</param>
    /// <param name="take">Número máximo de itens a serem retornados (padrão: 50)</param>
    /// <returns>ActionResult{List{ReadMovieDTO}}</returns>
    /// <response code="200">Caso a requisição seja bem sucedida</response>
    [HttpGet]
    public ActionResult<List<ReadMovieDTO>> GetCinemaPagination([FromQuery] int skip = 0, int take = 50) {
        IQueryable<Cinema> cines = _dbContext.Cinemas.Skip(skip).Take(take);

        List<ReadMovieDTO> readMovieDTOs = cines.AsEnumerable()
            .Select(movie => _mapper.Map<ReadMovieDTO>(movie))
            .ToList();
        return Ok(readMovieDTOs);
    }

    /// <summary>
    /// Obtém um cinema pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do cinema que deseja obter</param>
    /// <returns>ActionResult</returns>
    /// <response code="200">Caso a requisição seja bem sucedida</response>
    /// <response code="404">Caso nenhum cinema seja encontrado com o ID informado</response>
    [HttpGet("{id}")]
    public ActionResult GetCinemaById(int id) {
        Cinema? cine = _FindCinemaById(id);
        if (cine == null) {
            return NotFound($"O cinema com ID: {id}, não foi encontrado.");
        }
        ReadCinemaDTO cineDTO = _mapper.Map<ReadCinemaDTO>(cine);
        return Ok(cineDTO);
    }

    /// <summary>
    /// Atualiza um cinema pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do cinema que deseja atualizar</param>
    /// <param name="cineDTO">Objeto com os campos a serem atualizados do cinema</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    /// <response code="404">Caso nenhum cinema seja encontrado com o ID informado</response>
    [HttpPut("{id}")]
    public ActionResult UpdateCinema(int id, [FromBody] UpdateCinemaDTO cineDTO) {
        Cinema? cine = _FindCinemaById(id);
        if (cine == null)
            return NotFound($"O cinema com ID: {id}, não foi encontrado.");

        ValidationResult result = _Validation(cineDTO);

        if (result.IsValid) {
            _mapper.Map(cineDTO, cine);
            _dbContext.SaveChanges();
            return NoContent();
        }
        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Atualiza parcialmente um cinema existente pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do cinema que deseja atualizar parcialmente</param>
    /// <param name="patchCine">JsonPatchDocument contendo as atualizações a serem aplicadas</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização parcial seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nas atualizações</response>
    /// <response code="404">Caso nenhum cinema seja encontrado com o ID informado</response>
    [HttpPatch("{id}")]
    public ActionResult UpdatePatchMovie(int id, [FromBody] JsonPatchDocument<UpdateCinemaDTO> patchCine) {
        Cinema? cine = _FindCinemaById(id);
        if (cine == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        UpdateCinemaDTO cineForUpdate = _mapper.Map<UpdateCinemaDTO>(cine);

        patchCine.ApplyTo(cineForUpdate);

        ValidationResult result = _Validation(cineForUpdate);

        if (result.IsValid) {
            _mapper.Map(cineForUpdate, cine);
            _dbContext.SaveChanges();
            return NoContent();
        }

        List<string> errors = _ListErrors(result);
        return BadRequest(errors);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCinema(int id) {
        Cinema? cine = _FindCinemaById(id);

        if (cine == null) return NotFound($"O cinema com ID: {id}, não foi encontrado.");

        _dbContext.Cinemas.Remove(cine);
        _dbContext.SaveChanges();
        return NoContent();
    }

    private Cinema? _FindCinemaById(int id) {
        Cinema? cinema = _dbContext.Cinemas.FirstOrDefault(cine => cine.Id == id);
        return cinema;
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
