// automoveisVendasApi/Controllers/CarrosController.cs
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
    public class CarrosController : ControllerBase
    {
       
        private readonly ICarroRepository _repository;
        private readonly ILogger<CarrosController> _logger;

        public CarrosController(ICarroRepository repository, ILogger<CarrosController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

      
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CarroDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CarroDto>>> GetAll()
        {
            var carros = await _repository.GetAllAsync();
            return Ok(carros.Select(ToDto));
        }

     
        [HttpGet("disponiveis")]
        [ProducesResponseType(typeof(IEnumerable<CarroDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CarroDto>>> GetDisponiveis()
        {
            var carros = await _repository.GetDisponiveisAsync();
            return Ok(carros.Select(ToDto));
        }

       
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CarroDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CarroDto>> GetById(int id)
        {
            var carro = await _repository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException($"Carro {id} não encontrado.");

            return Ok(ToDto(carro));
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(CarroDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CarroDto>> Create(CreateCarroDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Modelo) || string.IsNullOrWhiteSpace(dto.Placa))
                throw new DomainException("Modelo e Placa são obrigatórios para cadastrar um carro.");

            var existente = await _repository.GetByPlacaAsync(dto.Placa);
            if (existente is not null)
                throw new ConflictException($"Já existe um carro cadastrado com a placa '{dto.Placa}'.");

            var carro = new Carro
            {
                Modelo = dto.Modelo,
                Marca = dto.Marca,
                Ano = dto.Ano,
                Valor = dto.Valor,
                Placa = dto.Placa,
                Vendido = false
            };

            await _repository.AddAsync(carro);

            _logger.LogInformation(
                "Carro {CarroId} (placa {Placa}) cadastrado com sucesso. TraceId: {TraceId}",
                carro.CarroId, carro.Placa, HttpContext.TraceIdentifier);

            return CreatedAtAction(nameof(GetById), new { id = carro.CarroId }, ToDto(carro));
        }

        private static CarroDto ToDto(Carro c) => new()
        {
            CarroId = c.CarroId,
            Modelo = c.Modelo,
            Marca = c.Marca,
            Ano = c.Ano,
            Valor = c.Valor,
            Placa = c.Placa,
            Vendido = c.Vendido
        };
    }
}