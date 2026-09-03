
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
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoRepository _pagamentoRepository;
      
        private readonly IRepository<Venda> _vendaRepository;
        private readonly ILogger<PagamentosController> _logger;

        public PagamentosController(
            IPagamentoRepository pagamentoRepository,
            IRepository<Venda> vendaRepository,
            ILogger<PagamentosController> logger)
        {
            _pagamentoRepository = pagamentoRepository;
            _vendaRepository = vendaRepository;
            _logger = logger;
        }

    
        [HttpGet("venda/{vendaId:int}")]
        [ProducesResponseType(typeof(IEnumerable<PagamentoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PagamentoDto>>> GetByVenda(int vendaId)
        {
            _ = await _vendaRepository.GetByIdAsync(vendaId)
                ?? throw new ResourceNotFoundException($"Venda {vendaId} não encontrada.");

            var pagamentos = await _pagamentoRepository.GetByVendaIdAsync(vendaId);
            return Ok(pagamentos.Select(ToDto));
        }

        [HttpPost]
        [ProducesResponseType(typeof(PagamentoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagamentoDto>> Create(CreatePagamentoDto dto)
        {
            if (dto.Valor <= 0)
                throw new DomainException("O valor do pagamento deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(dto.Tipo))
                throw new DomainException("O tipo do pagamento é obrigatório.");

            _ = await _vendaRepository.GetByIdAsync(dto.VendaId)
                ?? throw new ResourceNotFoundException($"Venda {dto.VendaId} não encontrada.");

            var pagamento = new Pagamento
            {
                VendaId = dto.VendaId,
                Tipo = dto.Tipo,
                Valor = dto.Valor,
                DataPagamento = dto.DataPagamento
            };

            await _pagamentoRepository.AddAsync(pagamento);

            _logger.LogInformation(
                "Pagamento {PagamentoId} registrado para a Venda {VendaId}. TraceId: {TraceId}",
                pagamento.PagamentoId, pagamento.VendaId, HttpContext.TraceIdentifier);

            return CreatedAtAction(nameof(GetByVenda), new { vendaId = pagamento.VendaId }, ToDto(pagamento));
        }

        private static PagamentoDto ToDto(Pagamento p) => new()
        {
            PagamentoId = p.PagamentoId,
            Tipo = p.Tipo,
            Valor = p.Valor,
            DataPagamento = p.DataPagamento
        };
    }
}