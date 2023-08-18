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
public class AddressController : Controller {
    private CineInfoContext _dbContext;
    private IMapper _mapper;
    private FindById<Address> _FindAddressById;
    private ListErrors _ListErrors;
    private Validation<IAddress> _Validation;

    public AddressController(CineInfoContext dbContext, IMapper mapper) {
        _dbContext = dbContext;
        _mapper = mapper;
        _FindAddressById = new FindById<Address>(_dbContext);
        _ListErrors = new ListErrors();
        _Validation = new Validation<IAddress>(new AddressValidatior());
    }

    /// <summary>
    /// Adiciona um novo endereço
    /// </summary>
    /// <param name="adressDTO">Objeto com os campos necessários para criação de um endereço</param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Caso a criação seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    [HttpPost]
    public ActionResult AddAdress([FromBody] CreateAddressDTO adressDTO) {
        var result = _Validation.Validate(adressDTO);

        if (result.IsValid) {
            Address address = _mapper.Map<Address>(adressDTO);
            _dbContext.Addresses.Add(address);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetCinemaById), new { id = address.Id }, address);
        }
        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Obtém uma lista paginada de endereços
    /// </summary>
    /// <param name="skip">Número de itens a serem ignorados (padrão: 0)</param>
    /// <param name="take">Número máximo de itens a serem retornados (padrão: 50)</param>
    /// <returns>ActionResult{List{ReadAddressDTO}}</returns>
    /// <response code="200">Caso a requisição seja bem sucedida</response>
    [HttpGet]
    public ActionResult<List<ReadAddressDTO>> GetAddressPagination([FromQuery] int skip = 0, int take = 50) {
        IQueryable<Address> addresses = _dbContext.Addresses.Skip(skip).Take(take);

        List<ReadAddressDTO> readAddressDTOs = addresses.AsEnumerable()
            .Select(movie => _mapper.Map<ReadAddressDTO>(movie))
            .ToList();
        return Ok(readAddressDTOs);
    }

    /// <summary>
    /// Obtém um endereço pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do endereço que deseja obter</param>
    /// <returns>ActionResult</returns>
    /// <response code="200">Caso a requisição seja bem sucedida</response>
    /// <response code="404">Caso nenhum endereço seja encontrado com o ID informado</response>
    [HttpGet("{id}")]
    public ActionResult GetCinemaById(int id) {
        Address? address = _FindAddressById.Find(id);
        if (address == null) {
            return NotFound($"O cinema com ID: {id}, não foi encontrado.");
        }
        ReadAddressDTO addressDTO = _mapper.Map<ReadAddressDTO>(address);
        return Ok(addressDTO);
    }

    /// <summary>
    /// Atualiza um endereço pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do endereço que deseja atualizar</param>
    /// <param name="addressDTO">Objeto com os campos a serem atualizados do endereço</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nos campos</response>
    /// <response code="404">Caso nenhum endereço seja encontrado com o ID informado</response>
    [HttpPut("{id}")]
    public ActionResult UpdateAddress(int id, [FromBody] UpdateAddressDTO addressDTO) {
        Address? address = _FindAddressById.Find(id);
        if (address == null)
            return NotFound($"O endereço com ID: {id}, não foi encontrado.");

        ValidationResult result = _Validation.Validate(addressDTO);

        if (result.IsValid) {
            _mapper.Map(addressDTO, address);
            _dbContext.SaveChanges();
            return NoContent();
        }
        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Atualiza parcialmente um endereço existente pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do endereço que deseja atualizar parcialmente</param>
    /// <param name="patchAddress">JsonPatchDocument contendo as atualizações a serem aplicadas</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a atualização parcial seja bem sucedida</response>
    /// <response code="400">Caso ocorra um erro de validação nas atualizações</response>
    /// <response code="404">Caso nenhum cinema seja encontrado com o ID informado</response>
    [HttpPatch("{id}")]
    public ActionResult UpdatePatchAddress(int id, [FromBody] JsonPatchDocument<UpdateAddressDTO> patchAddress) {
        Address? address = _FindAddressById.Find(id);
        if (address == null) return NotFound($"O filme com ID: {id}, não foi encontrado.");

        UpdateAddressDTO addressForUpdate = _mapper.Map<UpdateAddressDTO>(address);

        patchAddress.ApplyTo(addressForUpdate);

        ValidationResult result = _Validation.Validate(addressForUpdate);

        if (result.IsValid) {
            _mapper.Map(addressForUpdate, address);
            _dbContext.SaveChanges();
            return NoContent();
        }

        List<string> errors = _ListErrors.Generate(result);
        return BadRequest(errors);
    }

    /// <summary>
    /// Exclui um endereço pelo ID
    /// </summary>
    /// <param name="id">Identificador(ID) do endereço que deseja excluir</param>
    /// <returns>ActionResult</returns>
    /// <response code="204">Caso a exclusão seja bem sucedida</response>
    /// <response code="404">Caso nenhum endereço seja encontrado com o ID informado</response>
    [HttpDelete("{id}")]
    public ActionResult DeleteCinema(int id) {
        Address? address = _FindAddressById.Find(id);

        if (address == null) return NotFound($"O endereço com ID: {id}, não foi encontrado.");

        _dbContext.Addresses.Remove(address);
        _dbContext.SaveChanges();
        return NoContent();
    }
}
