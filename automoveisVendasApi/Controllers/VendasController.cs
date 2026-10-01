using Asp.Versioning;
using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Application.Interfaces;
using automoveisVendasApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace automoveisVendasApi.Controllers
{
    /// <summary>
    /// Vendas — único recurso versionado do CP5.
    /// 1.0 (obsoleta): GET lista devolve o array completo.
    /// 2.0 (atual):    GET lista devolve envelope paginado.
    /// As duas versões usam o MESMO IVendaService.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/[controller]")]                        // ?api-version= ou header X-Api-Version
    [Route("api/v{version:apiVersion}/[controller]")]  // /api/v1/vendas e /api/v2/vendas
    [Produces("application/json")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaService _vendaService;
        private readonly ILogger<VendasController> _logger;

        public VendasController(IVendaService vendaService, ILogger<VendasController> logger)
        {
            _vendaService = vendaService;
            _logger = logger;
        }

        /// <summary>[OBSOLETA] Lista todas as vendas (array, sem paginação).</summary>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(IEnumerable<VendaDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VendaDto>>> GetAll()
        {
            var vendas = await _vendaService.ListarAsync();
            return Ok(vendas);
        }

        /// <summary>Lista vendas paginadas (envelope com totais).</summary>
        /// <param name="page">Página, começando em 1 (padrão 1).</param>
        /// <param name="pageSize">Itens por página, de 1 a 100 (padrão 20).</param>
        [HttpGet]
        [MapToApiVersion("2.0")]
        [EnableRateLimiting(RateLimitingExtensions.LeituraPolicy)]
        [ProducesResponseType(typeof(PagedResult<VendaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<PagedResult<VendaDto>>> GetPaged(
            [FromQuery] long page = PageRequest.DefaultPage,
            [FromQuery] int pageSize = PageRequest.DefaultPageSize)
        {
            var resultado = await _vendaService.ListarPaginadoAsync(page, pageSize);
            return Ok(resultado);
        }

        /// <summary>Busca uma venda pelo id (1.0 e 2.0).</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(VendaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VendaDto>> GetById(int id)
        {
            var venda = await _vendaService.ObterPorIdAsync(id);
            return Ok(venda);
        }

        /// <summary>Registra uma venda (1.0 e 2.0). Limitado a 10 requisições/minuto por IP.</summary>
        [HttpPost]
        [EnableRateLimiting(RateLimitingExtensions.EscritaPolicy)]
        [ProducesResponseType(typeof(VendaDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
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
    }
}