using AutoMapper;
using CineInfo_API.Data.DTOs;
using CineInfo_API.Data;
using CineInfo_API.Models;
using CineInfo_API.Utilities;
using Microsoft.AspNetCore.Mvc;
using CineInfo_API.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;

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

    /// <summary>
    /// Adiciona uma nova sessão
    /// </summary>
    /// <param name="sessionDTO">Objeto com os campos necessários para criação de uma sessão</param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Caso a criação seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    /// <response code="404">Caso o cinema ou filme não seja encontrado</response>
    [HttpPost]
    public ActionResult AddSession([FromBody] InputSessionDTO sessionDTO) {
        ValidationResult result = _Validation.Validate(sessionDTO);
        if (result.IsValid) {
            Movie? movie = _dbContext.Movies.Find(sessionDTO.MovieId);
            if (movie == null)
                return NotFound($"O filme com ID: {sessionDTO.MovieId}, não encontrado");

            Cinema? Cine = _dbContext.Cinemas.Find(sessionDTO.CinemaId);
            if (Cine == null)
                return NotFound($"O cinema com ID: {sessionDTO.CinemaId}, não encontrado");

            Session session = _mapper.Map<Session>(sessionDTO);
            _dbContext.Sessions.Add(session);
            _dbContext.SaveChanges();

            ReadSessionDTO returnSession = _mapper.Map<ReadSessionDTO>(session);
            return CreatedAtAction(
                nameof(GetSessionById),
                new { id = returnSession.Id },
                returnSession
            );
        }
        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Retona todos as sessões do banco de dados
    /// </summary>
    /// <returns>ActionResult{List{ReadSessionDTO}}</returns>
    /// <response code="200">Retorna a lista de sessões com sucesso.</response>
    [HttpGet("all")]
    public ActionResult<List<ReadSessionDTO>> GetAllSessions() {
        Session[] sessions = _dbContext.Sessions.ToArray<Session>();

        List<ReadSessionDTO> readSessionDTOs = sessions.AsEnumerable()
            .Select(cine => _mapper.Map<ReadSessionDTO>(cine))
            .ToList();
        return Ok(readSessionDTOs);
    }

    /// <summary>
    /// Obtém uma lista paginada de sessões
    /// </summary>
    /// <param name="skip">Número de itens a serem ignorados (padrão: 0)</param>
    /// <param name="take">Número máximo de itens a serem retornados (padrão: 50)</param>
    /// <returns>ActionResult{List{ReadSessionDTO}}</returns>
    /// <response code="200">Caso a requisição seja bem sucedida</response>
    [HttpGet]
    public ActionResult<List<ReadSessionDTO>> GetSessionPagination(
        [FromQuery] int skip = 0, int take = 50
    ) {
        Session[] sessions = _dbContext.Sessions.Skip(skip).Take(take).ToArray();

        List<ReadSessionDTO> readSessionDTOs = sessions.AsEnumerable()
            .Select(session => _mapper.Map<ReadSessionDTO>(session))
            .ToList();
        return Ok(readSessionDTOs);
    }

    /// <summary>
    /// Obtém uma sessão pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) da sessão que deseja obter</param>
    /// <returns>ActionResult</returns>
    /// <response code="200">Caso a requisição seja bem sucedida</response>
    /// <response code="404">Caso nenhuma sessão seja encontrado com o ID informado</response>
    [HttpGet("{id}")]
    public ActionResult GetSessionById(int id) {
        Session? session = _FindSessionById.Find(id);
        if (session == null) {
            return NotFound($"A sessão com ID: {id}, não foi encontrada.");
        }
        ReadSessionDTO sessionDTO = _mapper.Map<ReadSessionDTO>(session);
        return Ok(sessionDTO);
    }

    /// <summary>
    /// Atualiza uma sessão pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) da sessão que deseja atualizar</param>
    /// <param name="sessionDTO">Objeto com os campos a serem atualizados da sessão</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    /// <response code="404">Caso algum dos ID informados não seja encontrado</response>
    [HttpPut("{id}")]
    public ActionResult UpdateSession(int id, [FromBody] InputSessionDTO sessionDTO) {
        Session? session = _FindSessionById.Find(id);
        if (session == null)
            return NotFound($"A sessão com ID: {id}, não foi encontrada.");

        ValidationResult result = _Validation.Validate(sessionDTO);

        if (result.IsValid) {
            Movie? movie = _dbContext.Movies.Find(sessionDTO.MovieId);
            if (movie == null)
                return NotFound($"O filme com ID: {sessionDTO.MovieId}, não encontrado");

            Cinema? Cine = _dbContext.Cinemas.Find(sessionDTO.CinemaId);
            if (Cine == null)
                return NotFound($"O cinema com ID: {sessionDTO.CinemaId}, não encontrado");

            _mapper.Map(sessionDTO, session);
            _dbContext.SaveChanges();
            return NoContent();
        }
        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Atualiza parcialmente uma sessão pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) da sessão que deseja atualizar parcialmente</param>
    /// <param name="patchSession">JsonPatchDocument contendo as atualizações a serem aplicadas</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização parcial seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nas atualizações</response>
    /// <response code="404">Caso algum dos ID informados não seja encontrado</response>
    [HttpPatch("{id}")]
    public ActionResult UpdatePatchSession(
        int id,
        [FromBody] JsonPatchDocument<InputSessionDTO> patchSession
    ) {
        Session? session = _FindSessionById.Find(id);
        if (session == null)
            return NotFound($"A sessão com ID: {id}, não foi encontrada.");

        InputSessionDTO sessionForUpdate = _mapper.Map<InputSessionDTO>(session);

        patchSession.ApplyTo(sessionForUpdate);

        ValidationResult result = _Validation.Validate(sessionForUpdate);

        if (result.IsValid) {
            Movie? movie = _dbContext.Movies.Find(sessionForUpdate.MovieId);
            if (movie == null)
                return NotFound($"O filme com ID: {sessionForUpdate.MovieId}, não encontrado");

            Cinema? Cine = _dbContext.Cinemas.Find(sessionForUpdate.CinemaId);
            if (Cine == null)
                return NotFound($"O cinema com ID: {sessionForUpdate.CinemaId}, não encontrado");

            _mapper.Map(sessionForUpdate, session);
            _dbContext.SaveChanges();
            return NoContent();
        }

        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Exclui uma sessão pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) da sessão que deseja excluir</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a exclusão seja bem sucedida</response>
    /// <response code="404">Caso nenhuma sessão seja encontrada com o ID informado</response>
    [HttpDelete("{id}")]
    public ActionResult DeleteSession(int id) {
        Session? session = _FindSessionById.Find(id);

        if (session == null)
            return NotFound($"A sessão com ID: {id}, não foi encontrada.");

        _dbContext.Sessions.Remove(session);
        _dbContext.SaveChanges();
        return NoContent();
    }
}
