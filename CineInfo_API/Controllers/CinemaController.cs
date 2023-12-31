﻿using AutoMapper;
using CineInfo_API.Data;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Models;
using CineInfo_API.Utilities;
using CineInfo_API.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineInfo_API.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : Controller {
    private CineInfoContext _dbContext;
    private IMapper _mapper;
    private FindById<Cinema> _FindCinemaById;
    private ListErrors _ListErrors;
    private Validation<InputCinemaDTO> _Validation;

    public CinemaController(CineInfoContext dbContext, IMapper mapper) {
        _dbContext = dbContext;
        _mapper = mapper;
        _FindCinemaById = new FindById<Cinema>(_dbContext);
        _ListErrors = new ListErrors();
        _Validation = new Validation<InputCinemaDTO>(new CinemaValidator());
    }

    /// <summary>
    /// Adiciona um novo cinema
    /// </summary>
    /// <param name="cinemaDTO">Objeto com os campos necessários para criação de um cinema</param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Caso a criação seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    /// <response code="404">Caso o endereço não seja encontrado.</response>
    [HttpPost]
    public ActionResult AddCinema([FromBody] InputCinemaDTO cinemaDTO) {
        ValidationResult result = _Validation.Validate(cinemaDTO);

        if (result.IsValid) {
            Address? address = _dbContext.Addresses.Find(cinemaDTO.AddressId);
            if (address != null) {
                Cinema cine = _mapper.Map<Cinema>(cinemaDTO);
                _dbContext.Cinemas.Add(cine);
                _dbContext.SaveChanges();
                ReadCinemaDTO returnCinema = _mapper.Map<ReadCinemaDTO>(cine);
                return CreatedAtAction(
                    nameof(GetCinemaById),
                    new { id = returnCinema.Id },
                    returnCinema
                );
            }
            return NotFound("Endereço não encontrado");
        }
        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Retona todos os cinemas do banco de dados
    /// </summary>
    /// <returns>ActionResult{List{ReadCinemaDTO}}</returns>
    /// <response code="200">Retorna a lista de cinemas com sucesso.</response>
    [HttpGet("all")]
    public ActionResult<List<ReadCinemaDTO>> GetAllCinemas() {
        Cinema[] cines = _dbContext.Cinemas.ToArray<Cinema>();

        List<ReadCinemaDTO> readCinemaDTOs = cines.AsEnumerable()
            .Select(cine => _mapper.Map<ReadCinemaDTO>(cine))
            .ToList();
        return Ok(readCinemaDTOs);
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
        Cinema[] cines = _dbContext.Cinemas.Skip(skip).Take(take).ToArray();

        List<ReadCinemaDTO> readCinemaDTOs = cines.AsEnumerable()
            .Select(cine => _mapper.Map<ReadCinemaDTO>(cine))
            .ToList();
        return Ok(readCinemaDTOs);
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
        Cinema? cine = _FindCinemaById.Find(id);
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
    public ActionResult UpdateCinema(int id, [FromBody] InputCinemaDTO cineDTO) {
        Cinema? cine = _FindCinemaById.Find(id);
        if (cine == null)
            return NotFound($"O cinema com ID: {id}, não foi encontrado.");

        ValidationResult result = _Validation.Validate(cineDTO);

        if (result.IsValid) {
            _mapper.Map(cineDTO, cine);
            _dbContext.SaveChanges();
            return NoContent();
        }
        List<string> errors = _ListErrors.Generate(result);
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
    public ActionResult UpdatePatchCinema(int id, [FromBody] JsonPatchDocument<InputCinemaDTO> patchCine) {
        Cinema? cine = _FindCinemaById.Find(id);
        if (cine == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        InputCinemaDTO cineForUpdate = _mapper.Map<InputCinemaDTO>(cine);

        patchCine.ApplyTo(cineForUpdate);

        ValidationResult result = _Validation.Validate(cineForUpdate);

        if (result.IsValid) {
            _mapper.Map(cineForUpdate, cine);
            _dbContext.SaveChanges();
            return NoContent();
        }

        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Exclui um cinema pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do cinema que deseja excluir</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a exclusão seja bem sucedida</response>
    /// <response code="404">Caso nenhum cinema seja encontrado com o ID informado</response>
    [HttpDelete("{id}")]
    public ActionResult DeleteCinema(int id) {
        Cinema? cine = _FindCinemaById.Find(id);

        if (cine == null) return NotFound($"O cinema com ID: {id}, não foi encontrado.");

        _dbContext.Cinemas.Remove(cine);
        _dbContext.SaveChanges();
        return NoContent();
    }
}
