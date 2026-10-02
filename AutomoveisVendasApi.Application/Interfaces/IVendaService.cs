using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Application.DTOs;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IVendaService
    {
        Task<VendaDto> CriarVendaAsync(CreateVendaDto dto);

        Task<IReadOnlyList<VendaDto>> ListarAsync();

        Task<PagedResult<VendaDto>> ListarPaginadoAsync(long page, int pageSize);

        Task<VendaDto> ObterPorIdAsync(int id);
    }
}
