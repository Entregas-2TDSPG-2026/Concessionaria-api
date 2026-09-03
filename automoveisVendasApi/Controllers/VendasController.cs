
using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace automoveisVendasApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IVendaService _vendaService;
        private readonly ILogger<VendasController> _logger;

        public VendasController(
            IVendaRepository vendaRepository,
            IVendaService vendaService,
            ILogger<VendasController> logger)
        {
            _vendaRepository = vendaRepository;
            _vendaService = vendaService;
            _logger = logger;
        }

       
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VendaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VendaDto>>> GetAll()
        {
            var vendas = await _vendaRepository.GetWithDetailsAsync();
            return Ok(vendas.Select(ToDto));
        }

       
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VendaDto>> GetById(int id)
        {
            var vendas = await _vendaRepository.GetWithDetailsAsync();
            var venda = vendas.FirstOrDefault(v => v.VendaId == id)
                ?? throw new ResourceNotFoundException($"Venda {id} não encontrada.");

            return Ok(ToDto(venda));
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(VendaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<VendaDto>> Create(CreateVendaDto dto)
        {
           
            _logger.LogInformation(
                "Iniciando registro de venda. TraceId: {TraceId}, ClienteId: {ClienteId}, CarroId: {CarroId}, MotoId: {MotoId}",
                HttpContext.TraceIdentifier, dto.ClienteId, dto.CarroId, dto.MotoId);

            var resultado = await _vendaService.CriarVendaAsync(dto);
            
            _logger.LogInformation(
                "Venda registrada com sucesso. TraceId: {TraceId}, VendaId: {VendaId}",
                HttpContext.TraceIdentifier, resultado.VendaId);

            return CreatedAtAction(nameof(GetById), new { id = resultado.VendaId }, resultado);
        }

        private static VendaDto ToDto(AutomoveisVendasApi.Domain.Entities.Venda v) => new()
        {
            VendaId = v.VendaId,
            DataVenda = v.DataVenda,
            ValorTotal = v.ValorTotal,
            Status = v.Status,
            Cliente = v.Cliente is null ? null : new ClienteDto
            {
                ClienteId = v.Cliente.ClienteId,
                Nome = v.Cliente.Nome,
                Email = v.Cliente.Email,
                Telefone = v.Cliente.Telefone
            },
            Carro = v.Carro is null ? null : new CarroDto
            {
                CarroId = v.Carro.CarroId,
                Modelo = v.Carro.Modelo,
                Marca = v.Carro.Marca,
                Ano = v.Carro.Ano,
                Valor = v.Carro.Valor,
                Placa = v.Carro.Placa,
                Vendido = v.Carro.Vendido
            },
            Moto = v.Moto is null ? null : new MotoDto
            {
                MotoId = v.Moto.MotoId,
                Modelo = v.Moto.Modelo,
                Marca = v.Moto.Marca,
                Ano = v.Moto.Ano,
                Valor = v.Moto.Valor,
                Vendida = v.Moto.Vendida
            },
            Pagamentos = v.Pagamentos.Select(p => new PagamentoDto
            {
                PagamentoId = p.PagamentoId,
                Tipo = p.Tipo,
                Valor = p.Valor,
                DataPagamento = p.DataPagamento
            }).ToList()
        };
    }
}