using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Application.DTOs;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IVendaService
    {
        Task<VendaDto> CriarVendaAsync(CreateVendaDto dto);

        /// <summary>Contrato v1 (obsoleto): lista completa, sem paginação.</summary>
        Task<IReadOnlyList<VendaDto>> ListarAsync();

        /// <summary>Contrato v2: envelope paginado. Valida page/pageSize (DomainException -> 400).</summary>
        Task<PagedResult<VendaDto>> ListarPaginadoAsync(long page, int pageSize);

        Task<VendaDto> ObterPorIdAsync(int id);
    }
}