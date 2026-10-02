using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IVendaRepository : IRepository<Venda>
    {
        Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId);
        Task<IEnumerable<Venda>> GetWithDetailsAsync();

        Task<PagedResult<Venda>> GetPagedAsync(PageRequest request);
    }
}
