using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IPagamentoRepository : IRepository<Pagamento>
    {
        Task<IEnumerable<Pagamento>> GetByVendaIdAsync(int vendaId);
    }
}
