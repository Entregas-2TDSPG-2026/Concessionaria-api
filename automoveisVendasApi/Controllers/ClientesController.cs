
using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace automoveisVendasApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
     
        private readonly IRepository<Cliente> _repository;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IRepository<Cliente> repository, ILogger<ClientesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

     
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
        {
            var clientes = await _repository.GetAllAsync();
            return Ok(clientes.Select(ToDto));
        }

      
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var cliente = await _repository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException($"Cliente {id} não encontrado.");

            return Ok(ToDto(cliente));
        }

       
        [HttpPost]
        [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClienteDto>> Create(CreateClienteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome) || string.IsNullOrWhiteSpace(dto.Email))
                throw new DomainException("Nome e Email são obrigatórios para cadastrar um cliente.");

            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone
            };

            await _repository.AddAsync(cliente);

            _logger.LogInformation(
                "Cliente {ClienteId} cadastrado com sucesso. TraceId: {TraceId}",
                cliente.ClienteId, HttpContext.TraceIdentifier);

            return CreatedAtAction(nameof(GetById), new { id = cliente.ClienteId }, ToDto(cliente));
        }

        private static ClienteDto ToDto(Cliente c) => new()
        {
            ClienteId = c.ClienteId,
            Nome = c.Nome,
            Email = c.Email,
            Telefone = c.Telefone
        };
    }
}