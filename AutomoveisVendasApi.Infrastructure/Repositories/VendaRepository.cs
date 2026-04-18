using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Repositories
{
    public class VendaRepository : Repository<Venda>, IVendaRepository
    {
        public VendaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId) =>
            await _dbSet
                .Where(v => v.ClienteId == clienteId)
                .Include(v => v.Pagamentos)
                .ToListAsync();

        public async Task<IEnumerable<Venda>> GetWithDetailsAsync() =>
            await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Carro)
                .Include(v => v.Moto)
                .Include(v => v.Pagamentos)
                .ToListAsync();
    }
}
