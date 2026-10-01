using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IVendaRepository : IRepository<Venda>
    {
        Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId);
        Task<IEnumerable<Venda>> GetWithDetailsAsync();

        /// <summary>Página já cortada no banco (Count + OrderBy + Skip + Take).</summary>
        Task<PagedResult<Venda>> GetPagedAsync(PageRequest request);
    }
}