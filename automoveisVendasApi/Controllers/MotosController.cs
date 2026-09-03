
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
    public class MotosController : ControllerBase
    {
        private readonly IMotoRepository _repository;
        private readonly ILogger<MotosController> _logger;

        public MotosController(IMotoRepository repository, ILogger<MotosController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MotoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MotoDto>>> GetAll()
        {
            var motos = await _repository.GetAllAsync();
            return Ok(motos.Select(ToDto));
        }

        [HttpGet("disponiveis")]
        [ProducesResponseType(typeof(IEnumerable<MotoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MotoDto>>> GetDisponiveis()
        {
            var motos = await _repository.GetDisponiveisAsync();
            return Ok(motos.Select(ToDto));
        }

      
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MotoDto>> GetById(int id)
        {
            var moto = await _repository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException($"Moto {id} não encontrada.");

            return Ok(ToDto(moto));
        }

       
        [HttpPost]
        [ProducesResponseType(typeof(MotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MotoDto>> Create(CreateMotoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Modelo))
                throw new DomainException("Modelo é obrigatório para cadastrar uma moto.");

            var moto = new Moto
            {
                Modelo = dto.Modelo,
                Marca = dto.Marca,
                Ano = dto.Ano,
                Valor = dto.Valor,
                Vendida = false
            };

            await _repository.AddAsync(moto);

            _logger.LogInformation(
                "Moto {MotoId} cadastrada com sucesso. TraceId: {TraceId}",
                moto.MotoId, HttpContext.TraceIdentifier);

            return CreatedAtAction(nameof(GetById), new { id = moto.MotoId }, ToDto(moto));
        }

        private static MotoDto ToDto(Moto m) => new()
        {
            MotoId = m.MotoId,
            Modelo = m.Modelo,
            Marca = m.Marca,
            Ano = m.Ano,
            Valor = m.Valor,
            Vendida = m.Vendida
        };
    }
}