using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Repositories
{
    public class PagamentoRepository : Repository<Pagamento>, IPagamentoRepository
    {
        public PagamentoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Pagamento>> GetByVendaIdAsync(int vendaId) =>
            await _dbSet.Where(p => p.VendaId == vendaId).ToListAsync();
    }
}
